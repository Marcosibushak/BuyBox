using Ibushak.Productos.Core.DomainModel.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ibushak.Productos.Core.DAL.Repositorios
{
    public class Repositorio<TEntity> : IRepositorio<TEntity> where TEntity : class
    {
        protected readonly DbContext context;

        public Repositorio(DbContext context)
        {
            this.context = context;
        }

        public IEnumerable<TEntity> buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().Where(predicate).AsEnumerable();
        }

        public TEntity obtenerDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public IEnumerable<TEntity> obtenerTodos()
        {
            return context.Set<TEntity>().AsEnumerable();
        }

        public void agregar(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        public void borrar(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);
        }

        public void actualizar(TEntity entity)
        {
            context.Entry(entity).State = EntityState.Modified;

        }

        public TEntity Obtener(string id)
        {
            return context.Set<TEntity>().Find(id);
        }
    }
}
