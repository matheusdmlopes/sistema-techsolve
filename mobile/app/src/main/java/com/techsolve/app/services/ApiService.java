package com.techsolve.app.services;

import com.techsolve.app.models.Categoria;
import com.techsolve.app.models.Chamado;
import com.techsolve.app.models.CriarChamadoRequest;
import com.techsolve.app.models.LoginRequest;
import com.techsolve.app.models.LoginResponse;
import com.techsolve.app.models.Usuario;

import java.util.List;

import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.POST;
import retrofit2.http.PUT;
import retrofit2.http.Path;

public interface ApiService {

    @POST("Auth/login")
    Call<LoginResponse> login(@Body LoginRequest loginRequest);

    @GET("Chamados")
    Call<List<Chamado>> getChamados();

    @GET("Chamados/{id}")
    Call<Chamado> getChamado(@Path("id") int id);

    @POST("Chamados")
    Call<Chamado> criarChamado(@Body CriarChamadoRequest request);

    @GET("Categorias")
    Call<List<Categoria>> getCategorias();

    @GET("Usuarios/{id}")
    Call<Usuario> getUsuario(@Path("id") int id);

    @PUT("Usuarios/{id}")
    Call<Usuario> atualizarUsuario(@Path("id") int id, @Body Usuario usuario);
}
