package com.techsolve.app.utils;

public class Formatacao {

    private Formatacao() {
    }

    /**
     * Converte a data enviada pela API ("2026-11-17T09:30:00") para o formato usado no Brasil
     * ("17/11/2026"). Se o texto não tiver esse formato, devolve vazio.
     */
    public static String data(String dataDaApi) {
        if (dataDaApi == null || !dataDaApi.matches("\\d{4}-\\d{2}-\\d{2}.*")) {
            return "";
        }
        return dataDaApi.substring(8, 10) + "/" + dataDaApi.substring(5, 7) + "/" + dataDaApi.substring(0, 4);
    }
}
