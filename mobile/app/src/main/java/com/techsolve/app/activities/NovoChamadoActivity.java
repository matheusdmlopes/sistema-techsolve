package com.techsolve.app.activities;

import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.ArrayAdapter;
import android.widget.ProgressBar;
import android.widget.Spinner;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.cardview.widget.CardView;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.textfield.TextInputEditText;
import com.techsolve.app.R;
import com.techsolve.app.models.AnalisePreliminarRequest;
import com.techsolve.app.models.AnalisePreliminarResponse;
import com.techsolve.app.models.Categoria;
import com.techsolve.app.models.Chamado;
import com.techsolve.app.models.CriarChamadoRequest;
import com.techsolve.app.models.Prioridade;
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
    private Spinner spinnerCategoria, spinnerPrioridade;
    private MaterialButton btnCriar, btnAnalisarIA;
    private ProgressBar progressBar, progressBarIA;
    private CardView cardSugestaoIA;
    private TextView tvSugestao, tvCategoriaSugerida, tvPrioridadeSugerida, tvPodeResolver;
    private TokenManager tokenManager;

    private List<Categoria> categorias = new ArrayList<>();
    private List<Prioridade> prioridades = new ArrayList<>();
    private AnalisePreliminarResponse analiseIA = null;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_novo_chamado);

        tokenManager = new TokenManager(this);

        setupViews();
        carregarCategorias();
        carregarPrioridades();
    }

    private void setupViews() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        toolbar.setNavigationOnClickListener(v -> finish());

        etTitulo = findViewById(R.id.etTitulo);
        etDescricao = findViewById(R.id.etDescricao);
        spinnerCategoria = findViewById(R.id.spinnerCategoria);
        spinnerPrioridade = findViewById(R.id.spinnerPrioridade);
        btnCriar = findViewById(R.id.btnCriar);
        btnAnalisarIA = findViewById(R.id.btnAnalisarIA);
        progressBar = findViewById(R.id.progressBar);
        progressBarIA = findViewById(R.id.progressBarIA);
        cardSugestaoIA = findViewById(R.id.cardSugestaoIA);
        tvSugestao = findViewById(R.id.tvSugestao);
        tvCategoriaSugerida = findViewById(R.id.tvCategoriaSugerida);
        tvPrioridadeSugerida = findViewById(R.id.tvPrioridadeSugerida);
        tvPodeResolver = findViewById(R.id.tvPodeResolver);

        btnAnalisarIA.setOnClickListener(v -> analisarComIA());
        btnCriar.setOnClickListener(v -> criarChamado());
    }

    private void carregarCategorias() {
        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

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

    private void carregarPrioridades() {
        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

        apiService.getPrioridades().enqueue(new Callback<List<Prioridade>>() {
            @Override
            public void onResponse(Call<List<Prioridade>> call, Response<List<Prioridade>> response) {
                if (response.isSuccessful() && response.body() != null) {
                    prioridades = response.body();

                    List<String> nomesPrioridades = new ArrayList<>();
                    for (Prioridade pri : prioridades) {
                        nomesPrioridades.add(pri.getNome());
                    }

                    ArrayAdapter<String> adapter = new ArrayAdapter<>(
                            NovoChamadoActivity.this,
                            android.R.layout.simple_spinner_item,
                            nomesPrioridades
                    );
                    adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spinnerPrioridade.setAdapter(adapter);

                    // Pré-selecionar "Média" (índice 1 geralmente)
                    if (prioridades.size() >= 2) {
                        spinnerPrioridade.setSelection(1);
                    }
                }
            }

            @Override
            public void onFailure(Call<List<Prioridade>> call, Throwable t) {
                Toast.makeText(NovoChamadoActivity.this, "Erro ao carregar prioridades", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void analisarComIA() {
        String titulo = etTitulo.getText().toString().trim();
        String descricao = etDescricao.getText().toString().trim();

        if (titulo.length() < 5) {
            Toast.makeText(this, "O título deve ter pelo menos 5 caracteres", Toast.LENGTH_SHORT).show();
            return;
        }

        if (descricao.length() < 10) {
            Toast.makeText(this, "A descrição deve ter pelo menos 10 caracteres", Toast.LENGTH_SHORT).show();
            return;
        }

        fecharTeclado();
        btnAnalisarIA.setEnabled(false);
        progressBarIA.setVisibility(View.VISIBLE);
        cardSugestaoIA.setVisibility(View.GONE);

        AnalisePreliminarRequest request = new AnalisePreliminarRequest(titulo, descricao);
        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

        apiService.analisarProblema(request).enqueue(new Callback<AnalisePreliminarResponse>() {
            @Override
            public void onResponse(Call<AnalisePreliminarResponse> call, Response<AnalisePreliminarResponse> response) {
                btnAnalisarIA.setEnabled(true);
                progressBarIA.setVisibility(View.GONE);

                if (response.isSuccessful() && response.body() != null) {
                    analiseIA = response.body();
                    exibirSugestaoIA();
                } else {
                    Toast.makeText(NovoChamadoActivity.this, "Erro ao analisar com IA", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<AnalisePreliminarResponse> call, Throwable t) {
                btnAnalisarIA.setEnabled(true);
                progressBarIA.setVisibility(View.GONE);
                Toast.makeText(NovoChamadoActivity.this, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_LONG).show();
            }
        });
    }

    // Fecha o teclado para a sugestão da IA aparecer inteira na tela
    private void fecharTeclado() {
        View campoEmUso = getCurrentFocus();
        if (campoEmUso != null) {
            InputMethodManager teclado = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
            teclado.hideSoftInputFromWindow(campoEmUso.getWindowToken(), 0);
        }
    }

    private void exibirSugestaoIA() {
        if (analiseIA == null) return;

        // Exibir o card de sugestão
        cardSugestaoIA.setVisibility(View.VISIBLE);

        // Exibir a sugestão
        tvSugestao.setText(analiseIA.getSugestaoSolucao());

        // Exibir categoria e prioridade sugeridas
        tvCategoriaSugerida.setText(analiseIA.getCategoriaSugerida().getNome());
        tvPrioridadeSugerida.setText(analiseIA.getPrioridadeSugerida().getNome());

        // Exibir badge se pode resolver sem técnico
        if (analiseIA.isPodeResolverSemTecnico()) {
            tvPodeResolver.setVisibility(View.VISIBLE);
        } else {
            tvPodeResolver.setVisibility(View.GONE);
        }

        // Pré-selecionar categoria sugerida
        int categoriaId = analiseIA.getCategoriaSugerida().getId();
        for (int i = 0; i < categorias.size(); i++) {
            if (categorias.get(i).getId() == categoriaId) {
                spinnerCategoria.setSelection(i);
                break;
            }
        }

        // Pré-selecionar prioridade sugerida
        int prioridadeId = analiseIA.getPrioridadeSugerida().getId();
        for (int i = 0; i < prioridades.size(); i++) {
            if (prioridades.get(i).getId() == prioridadeId) {
                spinnerPrioridade.setSelection(i);
                break;
            }
        }

        // Scroll suave até a sugestão
        cardSugestaoIA.post(() -> {
            cardSugestaoIA.getParent().requestChildFocus(cardSugestaoIA, cardSugestaoIA);
        });
    }

    private void criarChamado() {
        String titulo = etTitulo.getText().toString().trim();
        String descricao = etDescricao.getText().toString().trim();

        if (titulo.isEmpty() || descricao.isEmpty()) {
            Toast.makeText(this, "Preencha todos os campos", Toast.LENGTH_SHORT).show();
            return;
        }

        if (categorias.isEmpty() || prioridades.isEmpty()) {
            Toast.makeText(this, "Aguarde o carregamento dos dados", Toast.LENGTH_SHORT).show();
            return;
        }

        int categoriaPosition = spinnerCategoria.getSelectedItemPosition();
        int categoriaId = categorias.get(categoriaPosition).getId();

        int prioridadePosition = spinnerPrioridade.getSelectedItemPosition();
        int prioridadeId = prioridades.get(prioridadePosition).getId();

        btnCriar.setEnabled(false);
        progressBar.setVisibility(View.VISIBLE);

        CriarChamadoRequest request = new CriarChamadoRequest(titulo, descricao, categoriaId, prioridadeId);
        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

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
