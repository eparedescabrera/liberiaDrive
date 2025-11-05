$(document).ready(function () {

    // ==============================================
    // 🔁 Botón para actualizar el Data Warehouse
    // ==============================================
    $("#btnActualizarDW").on("click", function () {
        Swal.fire({
            title: "¿Actualizar Data Warehouse?",
            text: "Esto refrescará la información con los últimos datos del sistema.",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, actualizar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $.post("/ReportesDW/ActualizarDW", function (resp) {
                    if (resp.success) {
                        Swal.fire("✅ Éxito", resp.message, "success")
                            .then(() => {
                                cargarGraficos();
                            });
                    } else {
                        Swal.fire("❌ Error", resp.message, "error");
                    }
                }).fail(() => {
                    Swal.fire("❌ Error", "Error de conexión con el servidor.", "error");
                });
            }
        });
    });

    // ==============================================
    // 📊 Función para cargar todos los gráficos
    // ==============================================
    function cargarGraficos() {
        cargarGraficoInstructor();
        cargarGraficoEstado();
        cargarGraficoMes();
    }

    // ==============================================
    // 📈 Gráfico 1 - Sesiones por Instructor
    // ==============================================
    function cargarGraficoInstructor() {
        $.get("/ReportesDW/ObtenerResumenInstructor", function (data) {
            const nombres = data.map(x => x.Instructor);
            const totales = data.map(x => x.Total);

            new Chart(document.getElementById("chartInstructor"), {
                type: "bar",
                data: {
                    labels: nombres,
                    datasets: [{
                        label: "Sesiones",
                        data: totales,
                        borderWidth: 1
                    }]
                },
                options: {
                    plugins: { legend: { display: false } },
                    scales: {
                        y: { beginAtZero: true, ticks: { stepSize: 1 } }
                    }
                }
            });
        });
    }

    // ==============================================
    // 📊 Gráfico 2 - Sesiones por Estado
    // ==============================================
    function cargarGraficoEstado() {
        $.get("/ReportesDW/ObtenerResumenEstado", function (data) {
            const estados = data.map(x => x.Estado);
            const totales = data.map(x => x.Total);

            new Chart(document.getElementById("chartEstado"), {
                type: "doughnut",
                data: {
                    labels: estados,
                    datasets: [{
                        data: totales,
                        backgroundColor: ["#28a745", "#ffc107", "#dc3545", "#17a2b8"]
                    }]
                },
                options: {
                    plugins: {
                        legend: { position: "bottom" }
                    }
                }
            });
        });
    }

    // ==============================================
    // 📆 Gráfico 3 - Sesiones por Mes
    // ==============================================
    function cargarGraficoMes() {
        $.get("/ReportesDW/ObtenerResumenMes", function (data) {
            const meses = data.map(x => x.Mes);
            const totales = data.map(x => x.Total);

            new Chart(document.getElementById("chartMes"), {
                type: "line",
                data: {
                    labels: meses,
                    datasets: [{
                        label: "Sesiones",
                        data: totales,
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    plugins: {
                        legend: { display: false }
                    },
                    scales: {
                        y: { beginAtZero: true, ticks: { stepSize: 1 } }
                    }
                }
            });
        });
    }

    // 🚀 Cargar los gráficos al iniciar
    cargarGraficos();
});
