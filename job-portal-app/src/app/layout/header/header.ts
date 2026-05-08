import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  standalone:true,
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
isOpen: boolean=false;
authService=inject(AuthService);

get isAuthenticated()
{
  return this.authService.isAuthenticated()
}


logout()
{
  this.authService.logout();
}

}
