package com.techsolve.app.utils;

import android.content.Context;
import android.content.SharedPreferences;

import java.util.Locale;

import okhttp3.HttpUrl;

public class ServidorConfig {
    // Arquivo separado do TokenManager, para o endereço não ser apagado ao sair da conta
    private static final String PREF_NAME = "TechSolveServidor";
    private static final String KEY_ENDERECO = "endereco_servidor";

    // Endereço do computador visto pelo emulador do Android
    public static final String ENDERECO_PADRAO = "http://10.0.2.2:5080";
    private static final int PORTA_PADRAO = 5080;

    private SharedPreferences prefs;

    public ServidorConfig(Context context) {
        prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
    }

    public boolean isConfigurado() {
        return prefs.contains(KEY_ENDERECO);
    }

    public String getEndereco() {
        return prefs.getString(KEY_ENDERECO, ENDERECO_PADRAO);
    }

    // Recebe o endereço já ajustado por normalizar()
    public void salvarEndereco(String endereco) {
        prefs.edit().putString(KEY_ENDERECO, endereco).apply();
    }

    // URL base do Retrofit (precisa terminar com barra)
    public String getUrlApi() {
        return getEndereco() + "/api/";
    }

    /**
     * Ajusta o que foi digitado para o formato "http://servidor:porta".
     * Se faltar "http://", acrescenta; se faltar a porta, usa a 5080.
     * Retorna null quando o endereço é inválido.
     */
    public static String normalizar(String digitado) {
        if (digitado == null) {
            return null;
        }

        String texto = digitado.replaceAll("\\s+", "");
        if (texto.isEmpty()) {
            return null;
        }

        String minusculo = texto.toLowerCase(Locale.ROOT);
        if (!minusculo.startsWith("http://") && !minusculo.startsWith("https://")) {
            texto = "http://" + texto;
        }

        HttpUrl url = HttpUrl.parse(texto);
        if (url == null) {
            return null;
        }

        // Sem porta digitada, usa a porta da API (em HTTPS fica a 443)
        int porta = url.port();
        if (!url.isHttps() && !temPorta(texto)) {
            porta = PORTA_PADRAO;
        }

        // Endereço IPv6 precisa ficar entre colchetes
        String host = url.host();
        if (host.contains(":")) {
            host = "[" + host + "]";
        }

        // Só servidor e porta são guardados; o que vier depois (ex.: "/api") é descartado
        return url.scheme() + "://" + host + ":" + porta;
    }

    // Verifica se a porta foi digitada (ex.: "http://192.168.0.15:5080/api")
    private static boolean temPorta(String endereco) {
        String semProtocolo = endereco.substring(endereco.indexOf("://") + 3);
        String servidor = semProtocolo.split("[/?#]", 2)[0];
        return servidor.matches(".*:\\d+");
    }
}
