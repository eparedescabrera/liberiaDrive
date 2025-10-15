// ==================== FUNCIONES CLIENTES ====================

// ✅ Abrir modal dinámico por AJAX
function abrirModal(url, id, titulo) {
    const modalLabel = document.getElementById('clienteModalLabel');
    const modalBody = document.getElementById('clienteModalBody');
    const modal = new bootstrap.Modal(document.getElementById('clienteModal'));

    modalLabel.textContent = titulo;
    modalBody.innerHTML = `
        <div class="text-center p-4">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2">Cargando...</p>
        </div>`;

    // Si no hay ID (como en "Nuevo Cliente"), evita ?id=undefined
    const fullUrl = id ? `${url}?id=${id}` : url;

    fetch(fullUrl, {
        headers: { "X-Requested-With": "XMLHttpRequest" } // ⚡ Importante
    })
        .then(response => {
            if (!response.ok) throw new Error('Error al cargar vista');
            return response.text();
        })
        .then(html => {
            modalBody.innerHTML = html;
            modal.show();
        })
        .catch(err => {
            modalBody.innerHTML = `<div class="alert alert-danger">❌ ${err.message}</div>`;
        });
}

// ✅ Confirmación de eliminación con SweetAlert2
function confirmarEliminar(id, nombre, apellidos) {
    Swal.fire({
        title: '¿Eliminar cliente?',
        text: `Está a punto de eliminar a ${nombre} ${apellidos}. Esta acción no se puede deshacer.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/Clientes/Delete/' + id;
        }
    });
}

// ========================
// CREAR CLIENTE (AJAX)
// ========================
document.addEventListener('submit', function (e) {
    if (e.target.id === 'formCrearCliente') {
        e.preventDefault();
        const form = e.target;

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    Swal.fire('✅ Éxito', 'Cliente registrado correctamente', 'success');
                    const modal = bootstrap.Modal.getInstance(document.getElementById('clienteModal'));
                    modal.hide();
                    setTimeout(() => location.reload(), 800);
                } else {
                    Swal.fire('⚠️ Atención', data.message || 'Revisa los datos del formulario', 'warning');
                }
            })
            .catch(err => Swal.fire('❌ Error', err.message, 'error'));
    }
});

// ========================
// EDITAR CLIENTE (AJAX)
// ========================
document.addEventListener('submit', function (e) {
    if (e.target.id === 'formEditarCliente') {
        e.preventDefault();
        const form = e.target;

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    Swal.fire('✅ Éxito', 'Cliente actualizado correctamente', 'success');
                    const modal = bootstrap.Modal.getInstance(document.getElementById('clienteModal'));
                    modal.hide();
                    setTimeout(() => location.reload(), 800);
                } else {
                    Swal.fire('⚠️ Error', data.message || 'No se pudo actualizar el cliente', 'error');
                }
            })
            .catch(err => Swal.fire('❌ Error', err.message, 'error'));
    }
});
