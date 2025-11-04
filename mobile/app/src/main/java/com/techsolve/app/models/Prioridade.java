package com.techsolve.app.models;

import com.google.gson.annotations.SerializedName;

public class Prioridade {
    @SerializedName("idPrioridade")
    private int id;

    @SerializedName("nivel")
    private String nome;

    private String descricao;

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getNome() {
        return nome;
    }

    public void setNome(String nome) {
        this.nome = nome;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(String descricao) {
        this.descricao = descricao;
    }
}
