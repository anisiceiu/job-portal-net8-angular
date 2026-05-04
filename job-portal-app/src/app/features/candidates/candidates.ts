import { Component } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";

@Component({
  selector: 'app-candidates',
  imports: [Header, Footer],
  templateUrl: './candidates.html',
  styleUrl: './candidates.css',
})
export class Candidates {

}
