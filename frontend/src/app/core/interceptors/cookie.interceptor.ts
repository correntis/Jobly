import { HttpInterceptorFn } from '@angular/common/http';

export const cookieInterceptor: HttpInterceptorFn = (req, next) => {
  const clonedRequest = req.clone({ withCredentials: true });

  console.debug(clonedRequest);

  return next(clonedRequest);
};
