package com.techsolve.app.activities;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;
import com.techsolve.app.R;
import com.techsolve.app.dialogs.ServidorDialog;
import com.techsolve.app.models.LoginRequest;
import com.techsolve.app.models.LoginResponse;
import com.techsolve.app.services.ApiClient;
import com.techsolve.app.services.ApiService;
import com.techsolve.app.utils.ServidorConfig;
import com.techsolve.app.utils.TokenManager;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class LoginActivity extends AppCompatActivity {

    private TextInputEditText etEmail, etSenha;
    private MaterialButton btnLogin, btnServidor;
    private ProgressBar progressBar;
    private TokenManager tokenManager;
    private ServidorConfig servidorConfig;
    private ServidorDialog servidorDialog;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);

        tokenManager = new TokenManager(this);
        servidorConfig = new ServidorConfig(this);

        // Se já estiver logado, ir direto para a tela principal
        if (tokenManager.isLoggedIn()) {
            irParaChamados();
            return;
        }

        etEmail = findViewById(R.id.etEmail);
        etSenha = findViewById(R.id.etSenha);
        btnLogin = findViewById(R.id.btnLogin);
        btnServidor = findViewById(R.id.btnServidor);
        progressBar = findViewById(R.id.progressBar);

        btnLogin.setOnClickListener(v -> realizarLogin());
        btnServidor.setOnClickListener(v -> abrirConfiguracaoServidor());
        atualizarTextoServidor();

        // Na primeira abertura ainda não há servidor salvo: pedir o endereço
        if (!servidorConfig.isConfigurado()) {
            abrirConfiguracaoServidor();
        }
    }

    private void abrirConfiguracaoServidor() {
        servidorDialog = new ServidorDialog(this, this::atualizarTextoServidor);
        servidorDialog.mostrar();
    }

    private void atualizarTextoServidor() {
        if (servidorConfig.isConfigurado()) {
            btnServidor.setText("Servidor: " + servidorConfig.getEndereco());
        } else {
            btnServidor.setText("Servidor");
        }
    }

    private void realizarLogin() {
        String email = etEmail.getText().toString().trim();
        String senha = etSenha.getText().toString().trim();

        if (email.isEmpty() || senha.isEmpty()) {
            Toast.makeText(this, "Preencha todos os campos", Toast.LENGTH_SHORT).show();
            return;
        }

        btnLogin.setEnabled(false);
        progressBar.setVisibility(View.VISIBLE);

        LoginRequest request = new LoginRequest(email, senha);
        ApiService apiService = ApiClient.getClient(this, null).create(ApiService.class);

        apiService.login(request).enqueue(new Callback<LoginResponse>() {
            @Override
            public void onResponse(Call<LoginResponse> call, Response<LoginResponse> response) {
                btnLogin.setEnabled(true);
                progressBar.setVisibility(View.GONE);

                if (response.isSuccessful() && response.body() != null) {
                    LoginResponse loginResponse = response.body();

                    // Salvar token e dados do usuário
                    tokenManager.saveToken(loginResponse.getToken());
                    tokenManager.saveUser(loginResponse.getUsuario());

                    Toast.makeText(LoginActivity.this, "Login realizado com sucesso!", Toast.LENGTH_SHORT).show();
                    irParaChamados();
                } else {
                    Toast.makeText(LoginActivity.this, "Email ou senha incorretos", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<LoginResponse> call, Throwable t) {
                btnLogin.setEnabled(true);
                progressBar.setVisibility(View.GONE);
                Toast.makeText(LoginActivity.this, "Sem conexão com " + servidorConfig.getEndereco()
                        + ". Confira o endereço em \"Servidor\".", Toast.LENGTH_LONG).show();
            }
        });
    }

    private void irParaChamados() {
        Intent intent = new Intent(this, ChamadosActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    @Override
    protected void onDestroy() {
        // Fecha o diálogo do servidor se a tela for destruída com ele aberto
        if (servidorDialog != null) {
            servidorDialog.fechar();
        }
        super.onDestroy();
    }
}
