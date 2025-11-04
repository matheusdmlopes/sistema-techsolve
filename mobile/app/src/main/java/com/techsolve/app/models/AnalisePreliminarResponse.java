package com.techsolve.app.models;

public class AnalisePreliminarResponse {
    private String sugestaoSolucao;
    private Categoria categoriaSugerida;
    private Prioridade prioridadeSugerida;
    private String nivelConfianca;
    private boolean podeResolverSemTecnico;

    public String getSugestaoSolucao() {
        return sugestaoSolucao;
    }

    public void setSugestaoSolucao(String sugestaoSolucao) {
        this.sugestaoSolucao = sugestaoSolucao;
    }

    public Categoria getCategoriaSugerida() {
        return categoriaSugerida;
    }

    public void setCategoriaSugerida(Categoria categoriaSugerida) {
        this.categoriaSugerida = categoriaSugerida;
    }

    public Prioridade getPrioridadeSugerida() {
        return prioridadeSugerida;
    }

    public void setPrioridadeSugerida(Prioridade prioridadeSugerida) {
        this.prioridadeSugerida = prioridadeSugerida;
    }

    public String getNivelConfianca() {
        return nivelConfianca;
    }

    public void setNivelConfianca(String nivelConfianca) {
        this.nivelConfianca = nivelConfianca;
    }

    public boolean isPodeResolverSemTecnico() {
        return podeResolverSemTecnico;
    }

    public void setPodeResolverSemTecnico(boolean podeResolverSemTecnico) {
        this.podeResolverSemTecnico = podeResolverSemTecnico;
    }
}
