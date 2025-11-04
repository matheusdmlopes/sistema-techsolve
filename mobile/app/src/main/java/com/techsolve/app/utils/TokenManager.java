package com.techsolve.app.utils;

import android.content.Context;
import android.content.SharedPreferences;

import com.google.gson.Gson;
import com.techsolve.app.models.Usuario;

public class TokenManager {
    private static final String PREF_NAME = "TechSolvePrefs";
    private static final String KEY_TOKEN = "jwt_token";
    private static final String KEY_USER = "user_data";

    private SharedPreferences prefs;
    private Gson gson;

    public TokenManager(Context context) {
        prefs = context.getSharedPreferences(PREF_NAME, Context.MODE_PRIVATE);
        gson = new Gson();
    }

    public void saveToken(String token) {
        prefs.edit().putString(KEY_TOKEN, token).apply();
    }

    public String getToken() {
        return prefs.getString(KEY_TOKEN, null);
    }

    public void saveUser(Usuario usuario) {
        String userJson = gson.toJson(usuario);
        prefs.edit().putString(KEY_USER, userJson).apply();
    }

    public Usuario getUser() {
        String userJson = prefs.getString(KEY_USER, null);
        if (userJson != null) {
            return gson.fromJson(userJson, Usuario.class);
        }
        return null;
    }

    public boolean isLoggedIn() {
        return getToken() != null;
    }

    public void clearSession() {
        prefs.edit().clear().apply();
    }
}
