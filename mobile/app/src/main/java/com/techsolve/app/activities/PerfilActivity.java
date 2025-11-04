package com.techsolve.app.activities;

import android.content.Intent;
import android.os.Bundle;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.button.MaterialButton;
import com.techsolve.app.R;
import com.techsolve.app.models.Usuario;
import com.techsolve.app.utils.TokenManager;

public class PerfilActivity extends AppCompatActivity {

    private TextView tvNome, tvEmail, tvTelefone, tvSetor, tvPerfil;
    private MaterialButton btnLogout;
    private TokenManager tokenManager;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_perfil);

        tokenManager = new TokenManager(this);
        Usuario usuario = tokenManager.getUser();

        if (usuario == null) {
            irParaLogin();
            return;
        }

        setupViews();
        exibirDados(usuario);
    }

    private void setupViews() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        tvNome = findViewById(R.id.tvNome);
        tvEmail = findViewById(R.id.tvEmail);
        tvTelefone = findViewById(R.id.tvTelefone);
        tvSetor = findViewById(R.id.tvSetor);
        tvPerfil = findViewById(R.id.tvPerfil);
        btnLogout = findViewById(R.id.btnLogout);

        btnLogout.setOnClickListener(v -> {
            tokenManager.clearSession();
            irParaLogin();
        });

        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        bottomNavigation.setSelectedItemId(R.id.nav_perfil);
        bottomNavigation.setOnItemSelectedListener(item -> {
            int itemId = item.getItemId();
            if (itemId == R.id.nav_chamados) {
                finish();
                return true;
            }
            return false;
        });
    }

    private void exibirDados(Usuario usuario) {
        tvNome.setText(usuario.getNome());
        tvEmail.setText(usuario.getEmail());
        tvTelefone.setText(usuario.getTelefone() != null ? usuario.getTelefone() : "Não informado");
        tvSetor.setText(usuario.getNomeSetor());
        tvPerfil.setText(usuario.getNomePerfil());
    }

    private void irParaLogin() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }
}
