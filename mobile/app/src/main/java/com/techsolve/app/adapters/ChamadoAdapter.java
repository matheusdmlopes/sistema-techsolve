package com.techsolve.app.adapters;

import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.recyclerview.widget.RecyclerView;

import com.techsolve.app.R;
import com.techsolve.app.models.Chamado;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

public class ChamadoAdapter extends RecyclerView.Adapter<ChamadoAdapter.ChamadoViewHolder> {

    private List<Chamado> chamados = new ArrayList<>();
    private OnChamadoClickListener listener;

    public interface OnChamadoClickListener {
        void onChamadoClick(Chamado chamado);
    }

    public ChamadoAdapter(OnChamadoClickListener listener) {
        this.listener = listener;
    }

    public void setChamados(List<Chamado> chamados) {
        this.chamados = chamados;
        notifyDataSetChanged();
    }

    @NonNull
    @Override
    public ChamadoViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext())
                .inflate(R.layout.item_chamado, parent, false);
        return new ChamadoViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull ChamadoViewHolder holder, int position) {
        Chamado chamado = chamados.get(position);
        holder.bind(chamado);
    }

    @Override
    public int getItemCount() {
        return chamados.size();
    }

    class ChamadoViewHolder extends RecyclerView.ViewHolder {
        private TextView tvTitulo, tvStatus, tvCategoria, tvPrioridade, tvPrioridadeValor, tvData;

        public ChamadoViewHolder(@NonNull View itemView) {
            super(itemView);
            tvTitulo = itemView.findViewById(R.id.tvTitulo);
            tvStatus = itemView.findViewById(R.id.tvStatus);
            tvCategoria = itemView.findViewById(R.id.tvCategoria);
            tvPrioridade = itemView.findViewById(R.id.tvPrioridade);
            tvPrioridadeValor = itemView.findViewById(R.id.tvPrioridadeValor);
            tvData = itemView.findViewById(R.id.tvData);

            itemView.setOnClickListener(v -> {
                if (listener != null && getAdapterPosition() != RecyclerView.NO_POSITION) {
                    listener.onChamadoClick(chamados.get(getAdapterPosition()));
                }
            });
        }

        public void bind(Chamado chamado) {
            tvTitulo.setText(chamado.getTitulo());
            tvStatus.setText(chamado.getNomeStatus());
            tvCategoria.setText(chamado.getNomeCategoria());
            tvPrioridadeValor.setText(chamado.getNomePrioridade());

            // Formatar data
            try {
                String dataOriginal = chamado.getDataAbertura();
                if (dataOriginal != null && dataOriginal.length() >= 10) {
                    tvData.setText(dataOriginal.substring(0, 10));
                }
            } catch (Exception e) {
                tvData.setText("");
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
                        tvPrioridadeValor.setTextColor(Color.parseColor("#4CAF50"));
                        break;
                    case "média":
                        tvPrioridadeValor.setTextColor(Color.parseColor("#FF9800"));
                        break;
                    case "alta":
                        tvPrioridadeValor.setTextColor(Color.parseColor("#FF5722"));
                        break;
                    case "crítica":
                        tvPrioridadeValor.setTextColor(Color.parseColor("#F44336"));
                        break;
                    default:
                        tvPrioridadeValor.setTextColor(Color.parseColor("#757575"));
                        break;
                }
            }
        }
    }
}
