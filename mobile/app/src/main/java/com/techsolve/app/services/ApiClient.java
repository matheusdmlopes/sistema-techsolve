package com.techsolve.app.services;

import okhttp3.OkHttpClient;
import okhttp3.logging.HttpLoggingInterceptor;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

import java.util.concurrent.TimeUnit;

public class ApiClient {
    // Altere esta URL para o IP da sua máquina quando testar no celular
    // Ex: "http://192.168.0.100:7095/api/" ou use a API Azure em produção
    private static final String BASE_URL = "https://api-pimads-fghxhgerbkbqhsdx.brazilsouth-01.azurewebsites.net/api/";

    private static Retrofit retrofit = null;

    public static Retrofit getClient(String token) {
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

        retrofit = new Retrofit.Builder()
                .baseUrl(BASE_URL)
                .addConverterFactory(GsonConverterFactory.create())
                .client(httpClient.build())
                .build();

        return retrofit;
    }
}
