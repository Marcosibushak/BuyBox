using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios
{
    public interface IRepositorio<TEntity> where TEntity : class
    {
        TEntity Obtener(string id);

        IEnumerable<TEntity> obtenerTodos();

        IEnumerable<TEntity> buscar(Expression<Func<TEntity, bool>> predicate);

        TEntity obtenerDefault(Expression<Func<TEntity, bool>> predicate);

        void agregar(TEntity entity);

        void borrar(TEntity entity);

        void actualizar(TEntity entity);
    }
}
