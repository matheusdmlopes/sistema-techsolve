package com.techsolve.app.models;

public class Chamado {
    private int id;
    private String titulo;
    private String descricao;
    private String dataAbertura;
    private String dataFechamento;
    private int idCategoria;
    private String nomeCategoria;
    private int idPrioridade;
    private String nomePrioridade;
    private int idStatus;
    private String nomeStatus;
    private int idUsuarioCriador;
    private String nomeUsuarioCriador;
    private Integer idUsuarioTecnico;
    private String nomeUsuarioTecnico;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
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

    public String getDataAbertura() {
        return dataAbertura;
    }

    public void setDataAbertura(String dataAbertura) {
        this.dataAbertura = dataAbertura;
    }

    public String getDataFechamento() {
        return dataFechamento;
    }

    public void setDataFechamento(String dataFechamento) {
        this.dataFechamento = dataFechamento;
    }

    public int getIdCategoria() {
        return idCategoria;
    }

    public void setIdCategoria(int idCategoria) {
        this.idCategoria = idCategoria;
    }

    public String getNomeCategoria() {
        return nomeCategoria;
    }

    public void setNomeCategoria(String nomeCategoria) {
        this.nomeCategoria = nomeCategoria;
    }

    public int getIdPrioridade() {
        return idPrioridade;
    }

    public void setIdPrioridade(int idPrioridade) {
        this.idPrioridade = idPrioridade;
    }

    public String getNomePrioridade() {
        return nomePrioridade;
    }

    public void setNomePrioridade(String nomePrioridade) {
        this.nomePrioridade = nomePrioridade;
    }

    public int getIdStatus() {
        return idStatus;
    }

    public void setIdStatus(int idStatus) {
        this.idStatus = idStatus;
    }

    public String getNomeStatus() {
        return nomeStatus;
    }

    public void setNomeStatus(String nomeStatus) {
        this.nomeStatus = nomeStatus;
    }

    public int getIdUsuarioCriador() {
        return idUsuarioCriador;
    }

    public void setIdUsuarioCriador(int idUsuarioCriador) {
        this.idUsuarioCriador = idUsuarioCriador;
    }

    public String getNomeUsuarioCriador() {
        return nomeUsuarioCriador;
    }

    public void setNomeUsuarioCriador(String nomeUsuarioCriador) {
        this.nomeUsuarioCriador = nomeUsuarioCriador;
    }

    public Integer getIdUsuarioTecnico() {
        return idUsuarioTecnico;
    }

    public void setIdUsuarioTecnico(Integer idUsuarioTecnico) {
        this.idUsuarioTecnico = idUsuarioTecnico;
    }

    public String getNomeUsuarioTecnico() {
        return nomeUsuarioTecnico;
    }

    public void setNomeUsuarioTecnico(String nomeUsuarioTecnico) {
        this.nomeUsuarioTecnico = nomeUsuarioTecnico;
    }
}
