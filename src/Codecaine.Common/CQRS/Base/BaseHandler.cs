using Codecaine.Common.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.CQRS.Base
{
    /// <summary>
    /// BaseHandler is an abstract class that provides a template for handling asynchronous operations safely.
    /// It includes methods to handle core logic and manage exceptions, ensuring proper logging and rollback mechanisms.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the asynchronous operations.</typeparam>
    public abstract class BaseHandler<TResult>
    {
        private readonly ILogger _logger;

        protected BaseHandler(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Safely handles an asynchronous operation by executing the core logic and managing exceptions.
        /// This method ensures that if the operation is canceled, a rollback (if provided) is executed, and proper logging is performed.
        /// </summary>
        /// <param name="handleCoreAsync">
        /// A delegate representing the core asynchronous operation to execute.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests during the operation.
        /// </param>
        /// <param name="rollBackAsync">
        /// An optional delegate representing a rollback operation to be executed if the operation is canceled.
        /// </param>
        /// <param name="context">
        /// An optional context string for logging purposes to identify the operation's source or purpose.
        /// </param>
        /// <returns>
        /// A Task representing the asynchronous operation that resolves to the result of type <typeparamref name="TResult"/>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Thrown when the operation is canceled (either by the client or due to a timeout).
        /// </exception>
        /// <exception cref="ServiceInfrastructureException">
        /// Thrown when a <see cref="ServiceInfrastructureException"/> occurs.
        /// </exception>
        /// <exception cref="EnterpriseLibraryException">
        /// Thrown when an <see cref="EnterpriseLibraryException"/> occurs.
        /// </exception>
        /// <exception cref="ServiceApplicationException">
        /// Thrown when any other exceptions occur during the operation, after handling and logging them.
        /// </exception>
        protected async Task<TResult> HandleSafelyAsync(
            Func<Task<TResult>> handleCoreAsync,  CancellationToken? cancellationToken = null, Func<Task<TResult>>? rollBackAsync = null)
        {
            try
            {
                return await handleCoreAsync();
            }
            catch (OperationCanceledException ex) when (cancellationToken?.IsCancellationRequested == true)
            {
                _logger?.LogWarning(ex,"Operation was canceled.");

                // Execute rollback if it's provided
                if (rollBackAsync != null)
                {
                    _logger?.LogInformation("Executing rollback due to cancellation.");
                    await rollBackAsync();
                }

                throw new ApplicationLayerException(new Primitives.Errors.ApplicationError(this.GetType().FullName, ex));
            }
            catch (DomainException)
            {
                throw; // Re throw the original exception.
            }
            catch (InfrastructureException )
            {                
                throw; // Re throw the original exception.
            }
            catch (NotFoundException)
            {
                throw;
            }
            
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An ServiceApplicationException occurred.");
                throw new ApplicationLayerException(new Primitives.Errors.ApplicationError(this.GetType().FullName, ex));
            }
        }

        /// <summary>
        /// Safely handles an asynchronous operation by executing the core logic and managing exceptions.
        /// This method ensures that if the operation is canceled, a rollback (if provided) is executed, and proper logging is performed.
        /// </summary>
        /// <param name="handleCoreAsync">
        /// A delegate representing the core asynchronous operation to execute.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests during the operation.
        /// </param>
        /// <param name="rollBackAsync">
        /// An optional delegate representing a rollback operation to be executed if the operation is canceled.
        /// </param>
        /// <param name="context">
        /// An optional context string for logging purposes to identify the operation's source or purpose.
        /// </param>
        /// <returns>
        /// A Task representing the asynchronous operation.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Thrown when the operation is canceled (either by the client or due to a timeout).
        /// </exception>
        /// <exception cref="ServiceInfrastructureException">
        /// Thrown when a <see cref="ServiceInfrastructureException"/> occurs.
        /// </exception>
        /// <exception cref="EnterpriseLibraryException">
        /// Thrown when an <see cref="EnterpriseLibraryException"/> occurs.
        /// </exception>
        /// <exception cref="ServiceApplicationException">
        /// Thrown when any other exceptions occur during the operation, after handling and logging them.
        /// </exception>
        protected async Task HandleSafelyAsync(
            Func<Task> handleCoreAsync, CancellationToken? cancellationToken = null, Func<Task<TResult>>? rollBackAsync = null)
        {
            try
            {
                await handleCoreAsync();
            }
            catch (OperationCanceledException ex) when (cancellationToken?.IsCancellationRequested == true)
            {
                _logger?.LogWarning(ex,"Operation was canceled.");

                // Execute rollback if it's provided
                if (rollBackAsync != null)
                {
                    _logger?.LogInformation("Executing rollback due to cancellation.");
                    await rollBackAsync();
                }

                throw new ApplicationLayerException(new Primitives.Errors.ApplicationError(this.GetType().FullName, ex));
            }
            catch (DomainException)
            {
                throw; // Re throw the original exception.
            }
            catch (InfrastructureException )
            {               
                throw; // Re throw the original exception.
            }
           
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An ApplicationLayerException occurred.");
                throw new ApplicationLayerException(new Primitives.Errors.ApplicationError(this.GetType().FullName, ex));
            }
        }
    }
}
