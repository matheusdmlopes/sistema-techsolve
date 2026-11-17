package com.techsolve.app.models;

import com.google.gson.annotations.SerializedName;

public class Chamado {
    @SerializedName("idChamado")
    private int id;
    private String titulo;
    private String descricao;
    private String dataAbertura;
    private String dataFechamento;

    private CategoriaSimples categoria;
    private PrioridadeSimples prioridade;
    private StatusSimples status;
    private UsuarioSimples usuarioCriador;
    private UsuarioSimples usuarioTecnico;

    // Classes internas para objetos aninhados
    public static class CategoriaSimples {
        @SerializedName("idCategoria")
        private int id;
        private String nome;

        public int getId() { return id; }
        public String getNome() { return nome; }
    }

    public static class PrioridadeSimples {
        @SerializedName("idPrioridade")
        private int id;
        // A API envia o nome da prioridade no campo "nivel"
        @SerializedName(value = "nivel", alternate = {"nome"})
        private String nome;

        public int getId() { return id; }
        public String getNome() { return nome; }
    }

    public static class StatusSimples {
        @SerializedName("idStatus")
        private int id;
        private String nome;

        public int getId() { return id; }
        public String getNome() { return nome; }
    }

    public static class UsuarioSimples {
        @SerializedName("idUsuario")
        private int id;
        private String nome;

        public int getId() { return id; }
        public String getNome() { return nome; }
    }

    // Getters
    public int getId() {
        return id;
    }

    public String getTitulo() {
        return titulo;
    }

    public String getDescricao() {
        return descricao;
    }

    public String getDataAbertura() {
        return dataAbertura;
    }

    public String getDataFechamento() {
        return dataFechamento;
    }

    public int getIdCategoria() {
        return categoria != null ? categoria.getId() : 0;
    }

    public String getNomeCategoria() {
        return categoria != null ? categoria.getNome() : "";
    }

    public int getIdPrioridade() {
        return prioridade != null ? prioridade.getId() : 0;
    }

    public String getNomePrioridade() {
        return prioridade != null ? prioridade.getNome() : "";
    }

    public int getIdStatus() {
        return status != null ? status.getId() : 0;
    }

    public String getNomeStatus() {
        return status != null ? status.getNome() : "";
    }

    public int getIdUsuarioCriador() {
        return usuarioCriador != null ? usuarioCriador.getId() : 0;
    }

    public String getNomeUsuarioCriador() {
        return usuarioCriador != null ? usuarioCriador.getNome() : "";
    }

    public Integer getIdUsuarioTecnico() {
        return usuarioTecnico != null ? usuarioTecnico.getId() : null;
    }

    public String getNomeUsuarioTecnico() {
        return usuarioTecnico != null ? usuarioTecnico.getNome() : null;
    }

    // Setters (se necessário)
    public void setId(int id) {
        this.id = id;
    }

    public void setTitulo(String titulo) {
        this.titulo = titulo;
    }

    public void setDescricao(String descricao) {
        this.descricao = descricao;
    }

    public void setDataAbertura(String dataAbertura) {
        this.dataAbertura = dataAbertura;
    }

    public void setDataFechamento(String dataFechamento) {
        this.dataFechamento = dataFechamento;
    }
}
