// ========================
// CREAR INSTRUCTOR (AJAX)
// ========================
document.addEventListener('submit', function (e) {
    if (e.target.id === 'formCrearInstructor') {
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
                    Swal.fire('✅ Éxito', 'Instructor registrado correctamente', 'success');
                    const modal = bootstrap.Modal.getInstance(document.getElementById('clienteModal'));
                    modal.hide();
                    setTimeout(() => location.reload(), 900);
                } else {
                    Swal.fire('⚠️ Atención', data.message || 'Error al registrar el instructor', 'warning');
                }
            })
            .catch(err => Swal.fire('❌ Error', err.message, 'error'));
    }
});

// ========================
// EDITAR INSTRUCTOR (AJAX)
// ========================
document.addEventListener('submit', function (e) {
    if (e.target.id === 'formEditarInstructor') {
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
                    Swal.fire('✅ Éxito', 'Instructor actualizado correctamente', 'success');
                    const modal = bootstrap.Modal.getInstance(document.getElementById('clienteModal'));
                    modal.hide();
                    setTimeout(() => location.reload(), 900);
                } else {
                    Swal.fire('⚠️ Atención', data.message || 'Error al actualizar el instructor', 'warning');
                }
            })
            .catch(err => Swal.fire('❌ Error', err.message, 'error'));
    }
});
