import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-header',
  imports: [RouterLink, RouterLinkActive],
  standalone:true,
  templateUrl: './header.html',
  styleUrl: './header.css',
})
export class Header {
isOpen: boolean=false;

}
