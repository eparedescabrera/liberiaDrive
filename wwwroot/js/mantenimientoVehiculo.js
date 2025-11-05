$(document).ready(function () {

    // ======================================================
    // 🧾 DataTable
    // ======================================================
    $('#tablaMantenimientos').DataTable({
        language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' },
        pageLength: 10,
        responsive: true
    });

    // ======================================================
    // 🆕 CREAR MANTENIMIENTO
    // ======================================================
    window.abrirModalCrearMantenimiento = function () {
        $("#tituloModal").text("Registrar Mantenimiento de Vehículo");

        $("#contenidoModal").load("/MantenimientoVehiculo/Create", function () {
            $("#modalMantenimiento").modal("show");
            inicializarComponentes();
        });
    };

    // ======================================================
    // ✏️ EDITAR MANTENIMIENTO
    // ======================================================
    window.abrirModalEditar = function (id) {
        $("#tituloModal").text("Editar Mantenimiento de Vehículo");

        $("#contenidoModal").load("/MantenimientoVehiculo/Edit/" + id, function () {
            $("#modalMantenimiento").modal("show");
            inicializarComponentes();
        });
    };

    // ======================================================
    // 🗑️ ELIMINAR MANTENIMIENTO
    // ======================================================
    window.eliminarMantenimiento = function (id) {
        Swal.fire({
            title: "¿Eliminar mantenimiento?",
            text: "Esta acción no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $.post("/MantenimientoVehiculo/DeleteConfirmed/" + id, function (resp) {
                    if (resp.success) {
                        Swal.fire("✅ Eliminado", "El mantenimiento fue eliminado correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("❌ Error", resp.message, "error");
                    }
                });
            }
        });
    };

    // ======================================================
    // ⚙️ COMPONENTES REUTILIZABLES (Select2 + Flatpickr)
    // ======================================================
    function inicializarComponentes() {

        // 🔍 Select2 - Vehículos disponibles
        $('.select2').select2({
            theme: 'bootstrap-5',
            dropdownParent: $('#modalMantenimiento'),
            placeholder: 'Seleccione un vehículo',
            allowClear: true,
            ajax: {
                url: '/SesionPractica/BuscarVehiculos', // solo muestra disponibles
                dataType: 'json',
                delay: 250,
                data: params => ({ term: params.term }),
                processResults: data => ({ results: data })
            },
            width: '100%'
        });

        // 📅 Flatpickr
        flatpickr(".calendario", {
            dateFormat: "Y-m-d",
            altInput: true,
            altFormat: "l j \\de F \\de Y",
            locale: "es",
            disableMobile: true,
            theme: "material_blue"
        });

        // 🚀 CREAR MANTENIMIENTO
        $(document).off('submit', '#formCrearMantenimiento').on('submit', '#formCrearMantenimiento', function (e) {
            e.preventDefault();
            $.post('/MantenimientoVehiculo/Create', $(this).serialize())
                .done(resp => {
                    if (resp.success) {
                        Swal.fire("✅ Éxito", "Mantenimiento registrado correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("⚠️ Advertencia", resp.message, "warning");
                    }
                })
                .fail(() => Swal.fire("❌ Error", "No se pudo registrar el mantenimiento", "error"));
        });

        // 🚀 EDITAR MANTENIMIENTO
        $(document).off('submit', '#formEditarMantenimiento').on('submit', '#formEditarMantenimiento', function (e) {
            e.preventDefault();
            $.post('/MantenimientoVehiculo/Edit', $(this).serialize())
                .done(resp => {
                    if (resp.success) {
                        Swal.fire("✅ Éxito", "Mantenimiento actualizado correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("⚠️ Advertencia", resp.message, "warning");
                    }
                })
                .fail(() => Swal.fire("❌ Error", "No se pudo actualizar el mantenimiento", "error"));
        });
    }
});
