import { Component } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";

@Component({
  selector: 'app-job-details',
  imports: [Header, Footer],
  templateUrl: './job-details.html',
  styleUrl: './job-details.css',
})
export class JobDetails {

}
