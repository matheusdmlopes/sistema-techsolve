package com.techsolve.app.dialogs;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ProgressBar;
import android.widget.TextView;
import android.widget.Toast;

import androidx.appcompat.app.AlertDialog;

import com.google.android.material.button.MaterialButton;
import com.google.android.material.dialog.MaterialAlertDialogBuilder;
import com.google.android.material.textfield.TextInputEditText;
import com.google.android.material.textfield.TextInputLayout;
import com.techsolve.app.R;
import com.techsolve.app.services.ApiClient;
import com.techsolve.app.utils.ServidorConfig;

import java.io.IOException;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.Response;

// Diálogo para informar, testar e salvar o endereço do servidor (API)
public class ServidorDialog {

    public interface OnServidorSalvoListener {
        void onServidorSalvo();
    }

    private Context context;
    private ServidorConfig servidorConfig;
    private OnServidorSalvoListener listener;

    private AlertDialog dialog;
    private TextInputLayout tilServidor;
    private TextInputEditText etServidor;
    private MaterialButton btnTestarConexao;
    private ProgressBar progressTeste;
    private TextView tvResultadoTeste;
    private Call testeEmAndamento;

    public ServidorDialog(Context context, OnServidorSalvoListener listener) {
        this.context = context;
        this.servidorConfig = new ServidorConfig(context);
        this.listener = listener;
    }

    public void mostrar() {
        View view = LayoutInflater.from(context).inflate(R.layout.dialog_servidor, null);

        tilServidor = view.findViewById(R.id.tilServidor);
        etServidor = view.findViewById(R.id.etServidor);
        btnTestarConexao = view.findViewById(R.id.btnTestarConexao);
        progressTeste = view.findViewById(R.id.progressTeste);
        tvResultadoTeste = view.findViewById(R.id.tvResultadoTeste);

        // Endereço salvo ou, na primeira vez, o endereço padrão
        etServidor.setText(servidorConfig.getEndereco());
        btnTestarConexao.setOnClickListener(v -> testarConexao());

        dialog = new MaterialAlertDialogBuilder(context)
                .setTitle("Servidor")
                .setView(view)
                .setPositiveButton("Salvar", null)
                .setNegativeButton("Cancelar", null)
                .create();

        // Se o diálogo fechar no meio de um teste, o teste é cancelado
        dialog.setOnDismissListener(d -> {
            if (testeEmAndamento != null) {
                testeEmAndamento.cancel();
            }
        });

        dialog.show();

        // Clique definido depois do show() para o diálogo não fechar com endereço inválido
        dialog.getButton(AlertDialog.BUTTON_POSITIVE).setOnClickListener(v -> salvar());
    }

    public void fechar() {
        if (dialog != null && dialog.isShowing()) {
            dialog.dismiss();
        }
    }

    // Lê o campo e mostra nele o endereço já ajustado (com "http://" e porta)
    private String lerEndereco() {
        String endereco = ServidorConfig.normalizar(etServidor.getText().toString());

        if (endereco == null) {
            tilServidor.setError("Endereço inválido. Exemplo: http://192.168.0.15:5080");
            return null;
        }

        tilServidor.setError(null);
        etServidor.setText(endereco);
        etServidor.setSelection(endereco.length());
        return endereco;
    }

    private void testarConexao() {
        String endereco = lerEndereco();
        if (endereco == null) {
            return;
        }

        btnTestarConexao.setEnabled(false);
        progressTeste.setVisibility(View.VISIBLE);
        tvResultadoTeste.setVisibility(View.GONE);

        testeEmAndamento = ApiClient.testarConexao(endereco, new Callback() {
            @Override
            public void onResponse(Call call, Response response) {
                boolean sucesso = response.isSuccessful();
                int codigo = response.code();
                response.close();

                if (sucesso) {
                    mostrarResultado(call, true, "Conexão OK! O servidor respondeu em " + endereco + ".");
                } else {
                    mostrarResultado(call, false, "O servidor respondeu com erro (código " + codigo + ").\n"
                            + "Confira se este é o endereço da API do TechSolve.");
                }
            }

            @Override
            public void onFailure(Call call, IOException e) {
                String mensagem = "Não foi possível conectar a " + endereco + ".\n\n"
                        + "Confira se:\n"
                        + "• a API está aberta no computador;\n"
                        + "• o celular está na mesma rede Wi-Fi;\n"
                        + "• o endereço e a porta estão certos;\n"
                        + "• o firewall do computador libera a porta.";

                if (e.getMessage() != null) {
                    mensagem += "\n\nDetalhe: " + e.getMessage();
                }

                mostrarResultado(call, false, mensagem);
            }
        });
    }

    // A resposta do teste chega em outra thread; a tela só pode ser alterada na principal
    private void mostrarResultado(Call call, boolean sucesso, String mensagem) {
        tvResultadoTeste.post(() -> {
            if (call.isCanceled()) {
                return;
            }

            btnTestarConexao.setEnabled(true);
            progressTeste.setVisibility(View.GONE);
            tvResultadoTeste.setText(mensagem);
            tvResultadoTeste.setTextColor(Color.parseColor(sucesso ? "#4CAF50" : "#F44336"));
            tvResultadoTeste.setVisibility(View.VISIBLE);
        });
    }

    private void salvar() {
        String endereco = lerEndereco();
        if (endereco == null) {
            return;
        }

        servidorConfig.salvarEndereco(endereco);
        Toast.makeText(context, "Servidor salvo: " + endereco, Toast.LENGTH_SHORT).show();
        dialog.dismiss();

        if (listener != null) {
            listener.onServidorSalvo();
        }
    }
}
