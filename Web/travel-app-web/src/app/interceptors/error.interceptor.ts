import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { catchError, throwError } from "rxjs";

/**
 * Global error interceptor that catches all HTTP errors from API calls.
 * This acts as a global 'try-catch' for network requests in the Angular app.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unknown error occurred!';

      if (error.error instanceof ErrorEvent) {
        // Client-side error
        errorMessage = `Error: ${error.error.message}`;
      } else {
        // Server-side error
        // If the server returns a ProblemDetails object (from our GlobalExceptionHandlerMiddleware)
        errorMessage = error.error?.detail || error.error?.message || error.message;
        
        console.error(
          `Backend returned code ${error.status}, ` +
          `body was: `, error.error
        );
      }

      // For now, we log to console and re-throw the error
      return throwError(() => new Error(errorMessage));
    })
  );
};
