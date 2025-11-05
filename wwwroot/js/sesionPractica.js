$(document).ready(function () {

    // ===================================================
    // 🧾 Inicializar DataTable
    // ===================================================
    $('#tablaSesiones').DataTable({
        language: { url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json' },
        pageLength: 10
    });

    // ===================================================
    // 🆕 CREAR SESIÓN
    // ===================================================
    window.abrirModalCrearSesion = function () {
        $("#tituloModal").text("Registrar Sesión Práctica");

        $("#contenidoModal").load("/SesionPractica/Create", function () {
            $("#modalSesion").modal("show");
            inicializarComponentes();
        });
    };

    // ===================================================
    // ✏️ EDITAR SESIÓN
    // ===================================================
    window.abrirModalEditar = function (id) {
        $("#tituloModal").text("Editar Sesión Práctica");

        $("#contenidoModal").load("/SesionPractica/Edit/" + id, function () {
            $("#modalSesion").modal("show");
            inicializarComponentes();
        });
    };

    // ===================================================
    // 🗑️ ELIMINAR SESIÓN
    // ===================================================
    window.abrirModalEliminar = function (id) {
        Swal.fire({
            title: "¿Eliminar sesión?",
            text: "Esta acción no se puede deshacer.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $.post("/SesionPractica/DeleteConfirmed/" + id, function (resp) {
                    if (resp.success) {
                        Swal.fire("✅ Eliminada", "La sesión fue eliminada correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("❌ Error", resp.message, "error");
                    }
                });
            }
        });
    };

    // ===================================================
    // ⚙️ COMPONENTES REUTILIZABLES (para Create/Edit)
    // ===================================================
    function inicializarComponentes() {

        // ===================================================
        // 🔍 SELECT2 - CLIENTES
        // ===================================================
        $('#IdCliente').select2({
            theme: 'bootstrap-5',
            dropdownParent: $('#modalSesion'),
            placeholder: 'Seleccione un cliente',
            allowClear: true,
            ajax: {
                url: '/SesionPractica/BuscarClientes',
                dataType: 'json',
                delay: 250,
                data: params => ({ term: params.term }),
                processResults: data => ({ results: data })
            }
        });

        // ===================================================
        // 🔍 SELECT2 - INSTRUCTORES ACTIVOS
        // ===================================================
        $('#IdInstructor').select2({
            theme: 'bootstrap-5',
            dropdownParent: $('#modalSesion'),
            placeholder: 'Seleccione un instructor activo',
            allowClear: true,
            ajax: {
                url: '/SesionPractica/BuscarInstructoresActivos',
                dataType: 'json',
                delay: 250,
                data: params => ({ term: params.term }),
                processResults: data => ({ results: data })
            }
        });

        // ===================================================
        // 🔍 SELECT2 - VEHÍCULOS DISPONIBLES
        // ===================================================
        $('#IdVehiculo').select2({
            theme: 'bootstrap-5',
            dropdownParent: $('#modalSesion'),
            placeholder: 'Seleccione un vehículo',
            allowClear: true,
            ajax: {
                url: '/SesionPractica/BuscarVehiculos',
                dataType: 'json',
                delay: 250,
                data: params => ({ term: params.term }),
                processResults: data => ({ results: data })
            }
        });

        // ===================================================
        // 📅 FLATPICKR - CALENDARIO
        // ===================================================
        flatpickr(".calendario", {
            dateFormat: "Y-m-d",
            altInput: true,
            altFormat: "l j \\de F \\de Y",
            locale: "es",
            disableMobile: true,
            theme: "material_blue"
        });

        // ===================================================
        // 🚀 CREAR SESIÓN
        // ===================================================
        $(document).off('submit', '#formCrearSesion').on('submit', '#formCrearSesion', function (e) {
            e.preventDefault();
            $.post('/SesionPractica/Create', $(this).serialize())
                .done(resp => {
                    if (resp.success) {
                        Swal.fire("✅ Éxito", "Sesión creada correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("⚠️ Advertencia", resp.message, "warning");
                    }
                })
                .fail(() => Swal.fire("❌ Error", "No se pudo crear la sesión", "error"));
        });

        // ===================================================
        // 🚀 EDITAR SESIÓN
        // ===================================================
        $(document).off('submit', '#formEditarSesion').on('submit', '#formEditarSesion', function (e) {
            e.preventDefault();
            $.post('/SesionPractica/Edit', $(this).serialize())
                .done(resp => {
                    if (resp.success) {
                        Swal.fire("✅ Éxito", "Sesión actualizada correctamente", "success")
                            .then(() => location.reload());
                    } else {
                        Swal.fire("⚠️ Advertencia", resp.message, "warning");
                    }
                })
                .fail(() => Swal.fire("❌ Error", "No se pudo actualizar la sesión", "error"));
        });
    }

});
