package com.techsolve.app.services;

import android.content.Context;

import com.techsolve.app.utils.ServidorConfig;

import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.util.concurrent.TimeUnit;

public class ApiClient {

    public static Retrofit getClient(Context context, String token) {
        // O endereço do servidor é configurado no próprio app (tela de login > Servidor).
        // O cliente é montado a cada chamada, então sempre usa o endereço salvo no momento.
        String baseUrl = new ServidorConfig(context).getUrlApi();

        HttpLoggingInterceptor logging = new HttpLoggingInterceptor();
        logging.setLevel(HttpLoggingInterceptor.Level.BODY);

        OkHttpClient.Builder httpClient = new OkHttpClient.Builder()
                .connectTimeout(30, TimeUnit.SECONDS)
                .readTimeout(30, TimeUnit.SECONDS)
                .writeTimeout(30, TimeUnit.SECONDS)
                .addInterceptor(logging);

        // Adicionar token JWT se disponível
        if (token != null && !token.isEmpty()) {
            httpClient.addInterceptor(chain -> {
                okhttp3.Request original = chain.request();
                okhttp3.Request.Builder requestBuilder = original.newBuilder()
                        .header("Authorization", "Bearer " + token)
                        .method(original.method(), original.body());
                okhttp3.Request request = requestBuilder.build();
                return chain.proceed(request);
            });
        }

        return new Retrofit.Builder()
                .baseUrl(baseUrl)
                .addConverterFactory(GsonConverterFactory.create())
                .client(httpClient.build())
                .build();
    }

    // Testa se a API responde no endereço informado (GET /health).
    // O resultado chega no callback em uma thread de segundo plano.
    public static okhttp3.Call testarConexao(String endereco, okhttp3.Callback callback) {
        OkHttpClient httpClient = new OkHttpClient.Builder()
                .connectTimeout(5, TimeUnit.SECONDS)
                .readTimeout(5, TimeUnit.SECONDS)
                .build();

        okhttp3.Request request = new okhttp3.Request.Builder()
                .url(endereco + "/health")
                .build();

        okhttp3.Call call = httpClient.newCall(request);
        call.enqueue(callback);
        return call;
    }
}
