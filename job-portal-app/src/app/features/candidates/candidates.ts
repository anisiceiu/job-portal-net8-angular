import { Component } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-candidates',
  imports: [Header, Footer, RouterLink],
  templateUrl: './candidates.html',
  styleUrl: './candidates.css',
})
export class Candidates {

}
