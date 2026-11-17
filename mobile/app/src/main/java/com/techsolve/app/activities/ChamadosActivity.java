package com.techsolve.app.activities;

import android.content.Intent;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;
import androidx.swiperefreshlayout.widget.SwipeRefreshLayout;

import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.floatingactionbutton.FloatingActionButton;
import com.techsolve.app.R;
import com.techsolve.app.adapters.ChamadoAdapter;
import com.techsolve.app.models.Chamado;
import com.techsolve.app.models.Usuario;
import com.techsolve.app.services.ApiClient;
import com.techsolve.app.services.ApiService;
import com.techsolve.app.utils.TokenManager;

import java.util.List;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class ChamadosActivity extends AppCompatActivity implements ChamadoAdapter.OnChamadoClickListener {

    private RecyclerView rvChamados;
    private ProgressBar progressBar;
    private TextView tvEmpty;
    private SwipeRefreshLayout swipeRefresh;
    private ChamadoAdapter adapter;
    private TokenManager tokenManager;
    private Usuario usuario;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_chamados);

        tokenManager = new TokenManager(this);
        usuario = tokenManager.getUser();

        if (usuario == null) {
            // Se não houver usuário, voltar para o login
            irParaLogin();
            return;
        }

        setupViews();
        setupRecyclerView();
        carregarChamados();
    }

    private void setupViews() {
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        rvChamados = findViewById(R.id.rvChamados);
        progressBar = findViewById(R.id.progressBar);
        tvEmpty = findViewById(R.id.tvEmpty);
        swipeRefresh = findViewById(R.id.swipeRefresh);

        swipeRefresh.setOnRefreshListener(this::carregarChamados);

        FloatingActionButton fabNovo = findViewById(R.id.fabNovo);
        fabNovo.setOnClickListener(v -> {
            Intent intent = new Intent(this, NovoChamadoActivity.class);
            startActivity(intent);
        });

        BottomNavigationView bottomNavigation = findViewById(R.id.bottomNavigation);
        bottomNavigation.setSelectedItemId(R.id.nav_chamados);
        bottomNavigation.setOnItemSelectedListener(item -> {
            int itemId = item.getItemId();
            if (itemId == R.id.nav_perfil) {
                startActivity(new Intent(this, PerfilActivity.class));
                return true;
            }
            return false;
        });
    }

    private void setupRecyclerView() {
        adapter = new ChamadoAdapter(this);
        rvChamados.setLayoutManager(new LinearLayoutManager(this));
        rvChamados.setAdapter(adapter);
    }

    private void carregarChamados() {
        progressBar.setVisibility(View.VISIBLE);
        tvEmpty.setVisibility(View.GONE);

        ApiService apiService = ApiClient.getClient(this, tokenManager.getToken()).create(ApiService.class);

        apiService.getChamados().enqueue(new Callback<List<Chamado>>() {
            @Override
            public void onResponse(Call<List<Chamado>> call, Response<List<Chamado>> response) {
                progressBar.setVisibility(View.GONE);
                swipeRefresh.setRefreshing(false);

                if (response.isSuccessful() && response.body() != null) {
                    List<Chamado> todosChamados = response.body();

                    // Filtrar apenas chamados do usuário logado
                    List<Chamado> chamadosDoUsuario = new java.util.ArrayList<>();
                    for (Chamado chamado : todosChamados) {
                        if (chamado.getIdUsuarioCriador() == usuario.getId()) {
                            chamadosDoUsuario.add(chamado);
                        }
                    }

                    if (chamadosDoUsuario.isEmpty()) {
                        tvEmpty.setVisibility(View.VISIBLE);
                    } else {
                        adapter.setChamados(chamadosDoUsuario);
                    }
                } else {
                    Toast.makeText(ChamadosActivity.this, "Erro ao carregar chamados", Toast.LENGTH_SHORT).show();
                }
            }

            @Override
            public void onFailure(Call<List<Chamado>> call, Throwable t) {
                progressBar.setVisibility(View.GONE);
                swipeRefresh.setRefreshing(false);
                Toast.makeText(ChamadosActivity.this, "Erro de conexão: " + t.getMessage(), Toast.LENGTH_SHORT).show();
            }
        });
    }

    @Override
    public void onChamadoClick(Chamado chamado) {
        Intent intent = new Intent(this, DetalhesChamadoActivity.class);
        intent.putExtra("chamado_id", chamado.getId());
        startActivity(intent);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.menu_chamados, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == R.id.action_logout) {
            tokenManager.clearSession();
            irParaLogin();
            return true;
        }
        return super.onOptionsItemSelected(item);
    }

    private void irParaLogin() {
        Intent intent = new Intent(this, LoginActivity.class);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        startActivity(intent);
        finish();
    }

    @Override
    protected void onResume() {
        super.onResume();
        // Recarregar chamados quando voltar para a tela
        carregarChamados();
    }
}
