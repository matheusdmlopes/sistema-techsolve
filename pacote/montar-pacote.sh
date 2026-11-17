#!/usr/bin/env bash
# Monta o pacote offline do TechSolve para Windows (servidor + site), sem precisar de Windows.
#
# Uso:  ./pacote/montar-pacote.sh
# Saída: pacote/saida/TechSolve/  e  pacote/saida/TechSolve-Offline.zip
#
# O desktop (WinForms) só compila no Windows e o APK precisa do Android SDK.
# Se já existirem compilados, informe as pastas para entrarem no pacote:
#   DESKTOP_DIR=/caminho/para/bin/Release  APK=/caminho/app-debug.apk  ./pacote/montar-pacote.sh
# O pacote completo, com as três interfaces, é gerado pelo GitHub Actions
# (.github/workflows/pacote-offline.yml).
set -euo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SAIDA="$RAIZ/pacote/saida"
PASTA="$SAIDA/TechSolve"

rm -rf "$SAIDA"
mkdir -p "$PASTA"

publicar() {
    dotnet publish "$1" --configuration Release --runtime win-x64 --self-contained true \
        --output "$2" -p:DebugType=None -p:DebugSymbols=false --nologo
}

echo ">> Servidor (API)"
publicar "$RAIZ/api/PIM.Api/PIM.Api.csproj" "$PASTA/api"

echo ">> Site"
publicar "$RAIZ/web/PIM.Web/PIM.Web.csproj" "$PASTA/site"

# O pacote sai sem banco e sem anexos: o servidor cria tudo na primeira execução.
rm -f "$PASTA"/api/techsolve.db*
rm -rf "$PASTA/api/wwwroot/uploads"

cp "$RAIZ"/pacote/*.bat "$RAIZ/pacote/LEIA-ME.txt" "$PASTA/"

if [[ -n "${DESKTOP_DIR:-}" ]]; then
    echo ">> Desktop"
    mkdir -p "$PASTA/desktop"
    cp -r "$DESKTOP_DIR"/. "$PASTA/desktop/"
fi

if [[ -n "${APK:-}" ]]; then
    echo ">> Celular"
    mkdir -p "$PASTA/celular"
    cp "$APK" "$PASTA/celular/TechSolve.apk"
fi

if command -v zip >/dev/null 2>&1; then
    (cd "$SAIDA" && zip -qr TechSolve-Offline.zip TechSolve)
    echo ">> Pacote: $SAIDA/TechSolve-Offline.zip"
else
    echo ">> Pasta pronta: $PASTA (instale o 'zip' para gerar o .zip)"
fi
