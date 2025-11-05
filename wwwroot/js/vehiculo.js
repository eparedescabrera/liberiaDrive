// ===================================================
// ✅ vehiculos.js
// ===================================================

$(document).ready(function () {

    // ===============================================
    // 🔍 BUSCAR POR PLACA
    // ===============================================
    $("#btnBuscar").click(function () {
        const placa = $("#txtBuscarPlaca").val().trim();
        $.get("/Vehiculos/BuscarPorPlaca", { placa }, function (resp) {
            if (resp.success) {
                const tbody = $("#tbodyVehiculos");
                tbody.empty();

                if (!resp.data || resp.data.length === 0) {
                    tbody.append("<tr><td colspan='6' class='text-center'>No se encontraron resultados.</td></tr>");
                    return;
                }

                resp.data.forEach(v => {
                    tbody.append(`
                        <tr>
                            <td>${v.Marca}</td>
                            <td>${v.Modelo}</td>
                            <td>${v.Transmision}</td>
                            <td>${v.Placa}</td>
                            <td>${v.Estado}</td>
                            <td>
                                <button class="btn btn-warning btn-sm" onclick="abrirModalEditar(${v.IdVehiculo})">✏️</button>
                                <button class="btn btn-danger btn-sm" onclick="eliminarVehiculo(${v.IdVehiculo})">🗑️</button>
                            </td>
                        </tr>
                    `);
                });
            }
        });
    });

    // ===============================================
    // ➕ ABRIR MODAL CREAR
    // ===============================================
    $("#btnNuevo").click(function () {
        $("#tituloModal").text("Agregar Vehículo");
        $("#contenidoModal").load("/Vehiculos/Create", function () {
            $("#modalVehiculo").modal("show");
        });
    });

    // ===============================================
    // 💾 CREAR VEHÍCULO (POST)
    // ===============================================
    $(document).on("submit", "#formCrearVehiculo", function (e) {
        e.preventDefault();
        $.post("/Vehiculos/Create", $(this).serialize(), function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: "success",
                    title: "Vehículo registrado correctamente",
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#modalVehiculo").modal("hide");
                setTimeout(() => location.reload(), 1500);
            } else {
                Swal.fire("Error", resp.message || "No se pudo registrar el vehículo.", "error");
            }
        });
    });

    // ===============================================
    // ✏️ EDITAR VEHÍCULO (POST)
    // ===============================================
    $(document).on("submit", "#formEditarVehiculo", function (e) {
        e.preventDefault();
        $.post("/Vehiculos/Edit", $(this).serialize(), function (resp) {
            if (resp.success) {
                Swal.fire({
                    icon: "success",
                    title: "Vehículo actualizado correctamente",
                    showConfirmButton: false,
                    timer: 1500
                });
                $("#modalVehiculo").modal("hide");
                setTimeout(() => location.reload(), 1500);
            } else {
                Swal.fire("Error", resp.message || "No se pudo actualizar el vehículo.", "error");
            }
        });
    });
});

// ===================================================
// 🔹 FUNCIONES GLOBALES
// ===================================================

// Editar (carga el partial en modal)
function abrirModalEditar(id) {
    $("#tituloModal").text("Editar Vehículo");
    $("#contenidoModal").load("/Vehiculos/Edit/" + id, function () {
        $("#modalVehiculo").modal("show");
    });
}
// Ver detalles del vehículo
function abrirModalDetalles(id) {
    $("#tituloModal").text("Detalles del Vehículo");
    $("#contenidoModal").load("/Vehiculos/Details/" + id, function () {
        $("#modalVehiculo").modal("show");
    });
}

// Eliminar (SweetAlert + AJAX)
function eliminarVehiculo(id) {
    Swal.fire({
        title: '¿Está seguro?',
        text: 'El vehículo será eliminado permanentemente.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            $.post("/Vehiculos/DeleteConfirmed", { id }, function (resp) {
                if (resp.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Eliminado correctamente',
                        showConfirmButton: false,
                        timer: 1500
                    });
                    setTimeout(() => location.reload(), 1500);
                } else {
                    Swal.fire("Error", resp.message || "No se pudo eliminar el vehículo.", "error");
                }
            });
        }
    });
}
