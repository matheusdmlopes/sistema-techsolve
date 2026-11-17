#!/usr/bin/env python3
"""
Abre o aplicativo TechSolve em um emulador Android e usa as telas como uma pessoa usaria:
informa o servidor, entra, vê os chamados, abre um chamado novo com a triagem da IA e confere o perfil.
Cada tela é fotografada, para dar para ver como o aplicativo ficou.

Uso: teste-android.py <arquivo .apk> <pasta das fotos>

Precisa do adb, de um emulador ligado e do servidor do TechSolve aberto na porta 5080 do computador.
Para o emulador, o computador é o endereço 10.0.2.2.
"""
import json
import re
import subprocess
import sys
import time
import urllib.request
import xml.etree.ElementTree as ET
from pathlib import Path

PACOTE = "com.techsolve.app"
SERVIDOR_NO_COMPUTADOR = "http://127.0.0.1:5080"
SERVIDOR_NO_EMULADOR = "10.0.2.2"
EMAIL = "pedro.costa@empresa.com"
SENHA = "senha123"
# Sem acentos: o comando que digita no emulador só aceita letras simples.
TITULO = "Impressora do setor parada"
DESCRICAO = "A impressora do setor parou e os documentos ficam presos na fila"
DATA_BRASILEIRA = re.compile(r"^\d{2}/\d{2}/\d{4}$")

pasta_das_fotos = Path(".")


class Falha(Exception):
    pass


def adb(*argumentos):
    return subprocess.run(["adb", *argumentos], capture_output=True, timeout=180).stdout


def ler_tela():
    """Lista os elementos da janela que está na frente, com texto, identificador e posição."""
    for _ in range(15):
        adb("shell", "rm -f /sdcard/tela.xml; uiautomator dump /sdcard/tela.xml")
        try:
            return list(ET.fromstring(adb("exec-out", "cat", "/sdcard/tela.xml")).iter("node"))
        except ET.ParseError:
            time.sleep(1)
    raise Falha("Não foi possível ler a tela do emulador.")


def achar(elementos, identificador=None, texto=None):
    """Primeiro elemento com o identificador e, se informado, com o texto (sem diferenciar maiúsculas)."""
    for elemento in elementos:
        if identificador and not elemento.get("resource-id", "").endswith("/" + identificador):
            continue
        if texto and texto.lower() not in elemento.get("text", "").lower():
            continue
        return elemento
    return None


def todos(elementos, identificador):
    return [e for e in elementos if e.get("resource-id", "").endswith("/" + identificador)]


def posicao(elemento):
    return [int(numero) for numero in re.findall(r"\d+", elemento.get("bounds"))]


def tocar(elemento):
    x1, y1, x2, y2 = posicao(elemento)
    adb("shell", "input", "tap", str((x1 + x2) // 2), str((y1 + y2) // 2))


def esperar(o_que, identificador=None, texto=None, segundos=45):
    limite = time.time() + segundos
    while True:
        elementos = ler_tela()
        elemento = achar(elementos, identificador, texto)
        if elemento is not None:
            return elemento

        # Aviso do Android de que algum programa demorou a responder: escolher "Aguardar".
        aguardar = achar(elementos, "aerr_wait")
        if aguardar is not None:
            tocar(aguardar)

        if time.time() > limite:
            raise Falha(f"Não apareceu na tela: {o_que}")
        time.sleep(1)


def rolar_ate(identificador):
    """Rola a tela para baixo até o elemento aparecer inteiro o bastante para ser tocado."""
    largura, altura = [int(numero) for numero in re.findall(r"\d+", adb("shell", "wm", "size").decode())[-2:]]
    for _ in range(6):
        elemento = achar(ler_tela(), identificador)
        if elemento is not None:
            x1, y1, x2, y2 = posicao(elemento)
            if y2 - y1 >= 90:
                return elemento
        meio = str(largura // 2)
        adb("shell", "input", "swipe", meio, str(int(altura * 0.7)), meio, str(int(altura * 0.4)), "400")
        time.sleep(1)
    raise Falha(f"Não foi possível rolar a tela até: {identificador}")


def digitar(identificador, texto, conferir=True):
    tocar(esperar(f"o campo {identificador}", identificador))
    adb("shell", "input", "text", "'" + texto.replace(" ", "%s") + "'")
    if conferir:
        esperar(f"o texto digitado no campo {identificador}", identificador, texto)


def foto(nome):
    time.sleep(1.5)  # tempo para a tela terminar de desenhar
    (pasta_das_fotos / f"{nome}.png").write_bytes(adb("exec-out", "screencap", "-p"))
    print(f"Foto: {nome}.png", flush=True)


def conferir_datas(elementos, onde):
    datas = [e.get("text", "") for e in todos(elementos, "tvData")]
    if not datas or not all(DATA_BRASILEIRA.match(data) for data in datas):
        raise Falha(f"Data fora do formato dia/mês/ano em {onde}: {datas}")


def conferir_endereco_do_servidor(elementos):
    botao = achar(elementos, "btnServidor")
    if botao is not None and "10.0.2.2:5080" not in botao.get("text", ""):
        raise Falha(f"A tela de entrada não mostra o servidor salvo: {botao.get('text')!r}")


def servidor(caminho, corpo=None, token=None):
    cabecalhos = {"Content-Type": "application/json"}
    if token:
        cabecalhos["Authorization"] = "Bearer " + token
    dados = json.dumps(corpo).encode() if corpo is not None else None
    pedido = urllib.request.Request(SERVIDOR_NO_COMPUTADOR + caminho, data=dados, headers=cabecalhos)
    with urllib.request.urlopen(pedido, timeout=30) as resposta:
        return json.load(resposta)


def chamados_do_teste():
    """Chamados com o título usado no teste, lidos direto do servidor."""
    token = servidor("/api/Auth/login", {"email": EMAIL, "senha": SENHA})["token"]
    return [c for c in servidor("/api/Chamados", token=token) if c.get("titulo") == TITULO]


def percorrer_o_aplicativo(apk):
    if chamados_do_teste():
        raise Falha("O servidor já tem um chamado com o título do teste.")

    instalacao = adb("install", "-r", apk).decode()
    if "Success" not in instalacao:
        raise Falha(f"O aplicativo não instalou: {instalacao}")
    adb("shell", "wm", "dismiss-keyguard")
    adb("shell", "am", "start", "-W", "-n", f"{PACOTE}/.activities.LoginActivity")

    # Primeira abertura: o aplicativo pede o endereço do servidor, com o campo vazio.
    campo = esperar("a tela do servidor na primeira abertura", "etServidor")
    if "10.0.2.2" in campo.get("text", ""):
        raise Falha("O campo do servidor veio preenchido com o endereço do emulador.")
    esperar("a explicação de onde achar o endereço", texto='linha "Celular"')
    foto("01-servidor-na-primeira-abertura")

    digitar("etServidor", SERVIDOR_NO_EMULADOR)
    tocar(esperar("o botão Testar conexão", "btnTestarConexao"))
    esperar("a resposta do teste de conexão", "tvResultadoTeste", "Conexão OK")
    foto("02-servidor-conexao-ok")
    tocar(esperar("o botão Salvar", "button1"))

    # Entrada
    esperar("a tela de entrada", "btnLogin")
    conferir_endereco_do_servidor(ler_tela())
    digitar("etEmail", EMAIL)
    digitar("etSenha", SENHA, conferir=False)
    foto("03-entrada")
    tocar(esperar("o botão Entrar", "btnLogin"))

    # Lista de chamados
    esperar("a lista de chamados", "fabNovo")
    esperar("um chamado na lista", "tvTitulo")
    conferir_datas(ler_tela(), "Meus Chamados")
    foto("04-meus-chamados")

    # Detalhes de um chamado
    tocar(esperar("um chamado na lista", "tvTitulo"))
    esperar("os detalhes do chamado", "tvDescricao")
    conferir_datas(ler_tela(), "Detalhes do Chamado")
    foto("05-detalhes-do-chamado")
    adb("shell", "input", "keyevent", "4")  # botão Voltar

    # Chamado novo, com a triagem da IA
    tocar(esperar("o botão de chamado novo", "fabNovo"))
    digitar("etTitulo", TITULO)
    digitar("etDescricao", DESCRICAO)
    tocar(rolar_ate("btnAnalisarIA"))
    esperar("a categoria sugerida pela IA", "tvCategoriaSugerida", "impress")
    foto("06-chamado-novo-sugestao-da-ia")
    botao_criar = rolar_ate("btnCriar")
    foto("07-chamado-novo-categoria-e-prioridade")
    tocar(botao_criar)

    esperar("a lista de chamados depois de criar", "fabNovo")
    esperar("o chamado novo na lista", "tvTitulo", TITULO)
    foto("08-chamado-criado")
    criados = chamados_do_teste()
    if len(criados) != 1:
        raise Falha(f"O servidor deveria ter 1 chamado criado pelo aplicativo e tem {len(criados)}.")
    print("Chamado gravado no servidor: " + json.dumps(criados[0], ensure_ascii=False)[:600], flush=True)

    # Perfil
    tocar(esperar("a aba Perfil", "nav_perfil"))
    esperar("a tela de perfil", "btnLogout")
    esperar("o e-mail no perfil", "tvEmail", EMAIL)
    foto("09-perfil")

    # Com o celular no modo escuro, o aplicativo continua claro e legível.
    adb("shell", "cmd", "uimode", "night", "yes")
    time.sleep(3)
    esperar("a tela de perfil com o celular no modo escuro", "btnLogout")
    foto("10-perfil-celular-no-modo-escuro")
    tocar(esperar("a aba Chamados", "nav_chamados"))
    esperar("a lista de chamados com o celular no modo escuro", "fabNovo")
    esperar("um chamado na lista", "tvTitulo")
    foto("11-chamados-celular-no-modo-escuro")

    # Sair: volta para a entrada, sem pedir o servidor de novo.
    tocar(esperar("a aba Perfil", "nav_perfil"))
    tocar(esperar("o botão Sair", "btnLogout"))
    esperar("a tela de entrada depois de sair", "btnLogin")
    conferir_endereco_do_servidor(ler_tela())
    foto("12-saiu-da-conta")

    print("Aplicativo conferido: servidor, entrada, lista, detalhes, chamado novo com IA, perfil e saída.")


def principal():
    global pasta_das_fotos
    apk, pasta_das_fotos = sys.argv[1], Path(sys.argv[2])
    pasta_das_fotos.mkdir(parents=True, exist_ok=True)

    try:
        percorrer_o_aplicativo(apk)
    except Exception as erro:
        # Guarda a tela do momento da falha e o registro do Android, para descobrir a causa.
        print(f"FALHOU: {erro}", flush=True)
        (pasta_das_fotos / "falha-tela.png").write_bytes(adb("exec-out", "screencap", "-p"))
        adb("shell", "uiautomator", "dump", "/sdcard/falha.xml")
        (pasta_das_fotos / "falha-tela.xml").write_bytes(adb("exec-out", "cat", "/sdcard/falha.xml"))
        (pasta_das_fotos / "falha-registro-android.txt").write_bytes(adb("logcat", "-d", "-t", "4000"))
        sys.exit(1)


if __name__ == "__main__":
    principal()
