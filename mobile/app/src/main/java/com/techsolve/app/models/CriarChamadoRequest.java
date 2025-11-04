package com.techsolve.app.models;

public class CriarChamadoRequest {
    private String titulo;
    private String descricao;
    private int idCategoria;
    private int idPrioridade;

    public CriarChamadoRequest(String titulo, String descricao, int idCategoria, int idPrioridade) {
        this.titulo = titulo;
        this.descricao = descricao;
        this.idCategoria = idCategoria;
        this.idPrioridade = idPrioridade;
    }

    public String getTitulo() {
        return titulo;
    }

    public void setTitulo(String titulo) {
        this.titulo = titulo;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(String descricao) {
        this.descricao = descricao;
    }

    public int getIdCategoria() {
        return idCategoria;
    }

    public void setIdCategoria(int idCategoria) {
        this.idCategoria = idCategoria;
    }

    public int getIdPrioridade() {
        return idPrioridade;
    }

    public void setIdPrioridade(int idPrioridade) {
        this.idPrioridade = idPrioridade;
    }
}
