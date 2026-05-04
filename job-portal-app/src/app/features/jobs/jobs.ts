import { Component } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";

@Component({
  selector: 'app-jobs',
  imports: [Header, Footer],
  templateUrl: './jobs.html',
  styleUrl: './jobs.css',
})
export class Jobs {

}
