import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { ApiAuthorizationModule } from 'src/api-authorization/api-authorization.module';
import { AuthorizeGuard } from 'src/api-authorization/authorize.guard';
import { AuthorizeInterceptor } from 'src/api-authorization/authorize.interceptor';
import { LoginComponent } from '../api-authorization/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { EmptyProjectStateComponent } from './components/empty-project-state/empty-project-state.component';
import { CreateProjectComponent } from './components/create-project/create-project.component';
import { ActionButtonComponent } from './components/action-button/action-button.component';
import { AppIconButtonComponent } from './components/icon-button/icon-button.component';
import { EditProjectComponent } from './components/edit-project/edit-project.component';
import { DeleteProjectComponent } from './components/delete-project/delete-project.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    NavMenuComponent,
    EmptyProjectStateComponent,
    CreateProjectComponent,
    ActionButtonComponent,
    AppIconButtonComponent,
    EditProjectComponent,
    DeleteProjectComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    ApiAuthorizationModule,
    RouterModule.forRoot([
      // Public routes
      { path: 'login', component: LoginComponent },

      // Secure routes
      {
        path: '',
        canActivate: [AuthorizeGuard], // Protect all routes under this parent
        children: [
          { path: 'dashboard', component: DashboardComponent },
          { path: 'createproject', component: CreateProjectComponent },
          { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
        ]
      },
      { path: '**', redirectTo: '' } // Redirect unknown routes*/
    ])
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthorizeInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
