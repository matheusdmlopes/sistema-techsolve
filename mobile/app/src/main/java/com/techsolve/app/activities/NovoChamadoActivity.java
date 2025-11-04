package com.techsolve.app.activities;

import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ProgressBar;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.Spinner;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;
import com.techsolve.app.R;
import com.techsolve.app.models.Categoria;
import com.techsolve.app.models.Chamado;
import com.techsolve.app.models.CriarChamadoRequest;
import com.techsolve.app.services.ApiClient;
import com.techsolve.app.services.ApiService;
import com.techsolve.app.utils.TokenManager;

import java.util.ArrayList;
import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class NovoChamadoActivity extends AppCompatActivity {

    private TextInputEditText etTitulo, etDescricao;
    private Spinner spinnerCategoria;
    private RadioGroup rgPrioridade;
    private MaterialButton btnCriar;
    private ProgressBar progressBar;
    private TokenManager tokenManager;

    private List<Categoria> categorias = new ArrayList<>();
    private int prioridadeId = 2; // Média por padrão

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_novo_chamado);

        tokenManager = new TokenManager(this);

        setupViews();
        carregarCategorias();
    }

    private void setupViews() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        toolbar.setNavigationOnClickListener(v -> finish());

        etTitulo = findViewById(R.id.etTitulo);
        etDescricao = findViewById(R.id.etDescricao);
        spinnerCategoria = findViewById(R.id.spinnerCategoria);
        rgPrioridade = findViewById(R.id.rgPrioridade);
        btnCriar = findViewById(R.id.btnCriar);
        progressBar = findViewById(R.id.progressBar);

        rgPrioridade.setOnCheckedChangeListener((group, checkedId) -> {
            if (checkedId == R.id.rbBaixa) {
                prioridadeId = 1;
            } else if (checkedId == R.id.rbMedia) {
                prioridadeId = 2;
            } else if (checkedId == R.id.rbAlta) {
                prioridadeId = 3;
            } else if (checkedId == R.id.rbCritica) {
                prioridadeId = 4;
            }
        });

        btnCriar.setOnClickListener(v -> criarChamado());
    }

    private void carregarCategorias() {
        ApiService apiService = ApiClient.getClient(tokenManager.getToken()).create(ApiService.class);

        apiService.getCategorias().enqueue(new Callback<List<Categoria>>() {
            @Override
            public void onResponse(Call<List<Categoria>> call, Response<List<Categoria>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    categorias = response.body();

                    List<String> nomesCategorias = new ArrayList<>();
                    for (Categoria cat : categorias) {
                        nomesCategorias.add(cat.getNome());
                    }

                    ArrayAdapter<String> adapter = new ArrayAdapter<>(
                            NovoChamadoActivity.this,
                            android.R.layout.simple_spinner_item,
                            nomesCategorias
                    );
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerCategoria.setAdapter(adapter);
                }
            }

            @Override
            public void onFailure(Call<List<Categoria>> call, Throwable t) {
                Toast.makeText(NovoChamadoActivity.this, "Erro ao carregar categorias", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void criarChamado() {
        String titulo = etTitulo.getText().toString().trim();
        String descricao = etDescricao.getText().toString().trim();

        if (titulo.isEmpty() || descricao.isEmpty()) {
            Toast.makeText(this, "Preencha todos os campos", Toast.LENGTH_SHORT).show();
            return;
        }

        if (categorias.isEmpty()) {
            Toast.makeText(this, "Aguarde o carregamento das categorias", Toast.LENGTH_SHORT).show();
            return;
        }

        int categoriaPosition = spinnerCategoria.getSelectedItemPosition();
        int categoriaId = categorias.get(categoriaPosition).getId();

        btnCriar.setEnabled(false);
        progressBar.setVisibility(View.VISIBLE);

        CriarChamadoRequest request = new CriarChamadoRequest(titulo, descricao, categoriaId, prioridadeId);
        ApiService apiService = ApiClient.getClient(tokenManager.getToken()).create(ApiService.class);

        apiService.criarChamado(request).enqueue(new Callback<Chamado>() {
            @Override
            public void onResponse(Call<Chamado> call, Response<Chamado> response) {
                btnCriar.setEnabled(true);
                progressBar.setVisibility(View.GONE);

                if (response.isSuccessful()) {
                    Toast.makeText(NovoChamadoActivity.this, "Chamado criado com sucesso!", Toast.LENGTH_SHORT).show();
                    finish();
                } else {
                    Toast.makeText(NovoChamadoActivity.this, "Erro ao criar chamado", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<Chamado> call, Throwable t) {
                btnCriar.setEnabled(true);
                progressBar.setVisibility(View.GONE);
                Toast.makeText(NovoChamadoActivity.this, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
