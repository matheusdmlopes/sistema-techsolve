package com.techsolve.app.activities;

import android.graphics.Color;
import android.os.Bundle;
import android.view.View;
import android.widget.LinearLayout;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import com.techsolve.app.R;
import com.techsolve.app.models.Chamado;
import com.techsolve.app.services.ApiClient;
import com.techsolve.app.services.ApiService;
import com.techsolve.app.utils.TokenManager;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class DetalhesChamadoActivity extends AppCompatActivity {

    private TextView tvTitulo, tvStatus, tvData, tvDescricao, tvCategoria, tvPrioridade, tvTecnico;
    private LinearLayout layoutContent, layoutTecnico;
    private ProgressBar progressBar;
    private TokenManager tokenManager;
    private int chamadoId;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_detalhes_chamado);

        tokenManager = new TokenManager(this);
        chamadoId = getIntent().getIntExtra("chamado_id", -1);

        if (chamadoId == -1) {
            Toast.makeText(this, "Erro ao carregar chamado", Toast.LENGTH_SHORT).show();
            finish();
            return;
        }

        setupViews();
        carregarChamado();
    }

    private void setupViews() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        toolbar.setNavigationOnClickListener(v -> finish());

        tvTitulo = findViewById(R.id.tvTitulo);
        tvStatus = findViewById(R.id.tvStatus);
        tvData = findViewById(R.id.tvData);
        tvDescricao = findViewById(R.id.tvDescricao);
        tvCategoria = findViewById(R.id.tvCategoria);
        tvPrioridade = findViewById(R.id.tvPrioridade);
        tvTecnico = findViewById(R.id.tvTecnico);
        layoutContent = findViewById(R.id.layoutContent);
        layoutTecnico = findViewById(R.id.layoutTecnico);
        progressBar = findViewById(R.id.progressBar);
    }

    private void carregarChamado() {
        progressBar.setVisibility(View.VISIBLE);
        layoutContent.setVisibility(View.GONE);

        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

        apiService.getChamado(chamadoId).enqueue(new Callback<Chamado>() {
            @Override
            public void onResponse(Call<Chamado> call, Response<Chamado> response) {
                progressBar.setVisibility(View.GONE);

                if (response.isSuccessful() && response.body() != null) {
                    Chamado chamado = response.body();
                    exibirChamado(chamado);
                    layoutContent.setVisibility(View.VISIBLE);
                } else {
                    Toast.makeText(DetalhesChamadoActivity.this, "Erro ao carregar chamado", Toast.LENGTH_SHORT).show();
                    finish();
                }
            }

            @Override
            public void onFailure(Call<Chamado> call, Throwable t) {
                progressBar.setVisibility(View.GONE);
                Toast.makeText(DetalhesChamadoActivity.this, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }

    private void exibirChamado(Chamado chamado) {
        tvTitulo.setText(chamado.getTitulo());
        tvStatus.setText(chamado.getNomeStatus());
        tvDescricao.setText(chamado.getDescricao());
        tvCategoria.setText(chamado.getNomeCategoria());
        tvPrioridade.setText(chamado.getNomePrioridade());

        // Formatar data
        try {
            String dataOriginal = chamado.getDataAbertura();
            if (dataOriginal != null && dataOriginal.length() >= 10) {
                tvData.setText(dataOriginal.substring(0, 10));
            }
        } catch (Exception e) {
            tvData.setText("");
        }

        // Exibir técnico se houver
        if (chamado.getNomeUsuarioTecnico() != null && !chamado.getNomeUsuarioTecnico().isEmpty()) {
            tvTecnico.setText(chamado.getNomeUsuarioTecnico());
            layoutTecnico.setVisibility(View.VISIBLE);
        }

        // Cores do status
        String status = chamado.getNomeStatus();
        if (status != null) {
            switch (status.toLowerCase()) {
                case "aberto":
                    tvStatus.setBackgroundColor(Color.parseColor("#4CAF50"));
                    break;
                case "em andamento":
                    tvStatus.setBackgroundColor(Color.parseColor("#2196F3"));
                    break;
                case "resolvido":
                    tvStatus.setBackgroundColor(Color.parseColor("#9C27B0"));
                    break;
                case "fechado":
                    tvStatus.setBackgroundColor(Color.parseColor("#607D8B"));
                    break;
                case "cancelado":
                    tvStatus.setBackgroundColor(Color.parseColor("#F44336"));
                    break;
                default:
                    tvStatus.setBackgroundColor(Color.parseColor("#757575"));
                    break;
            }
        }

        // Cores da prioridade
        String prioridade = chamado.getNomePrioridade();
        if (prioridade != null) {
            switch (prioridade.toLowerCase()) {
                case "baixa":
                    tvPrioridade.setTextColor(Color.parseColor("#4CAF50"));
                    break;
                case "média":
                    tvPrioridade.setTextColor(Color.parseColor("#FF9800"));
                    break;
                case "alta":
                    tvPrioridade.setTextColor(Color.parseColor("#FF5722"));
                    break;
                case "crítica":
                    tvPrioridade.setTextColor(Color.parseColor("#F44336"));
                    break;
                default:
                    tvPrioridade.setTextColor(Color.parseColor("#757575"));
                    break;
            }
        }
    }
}
