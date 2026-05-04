import { Component } from '@angular/core';
import { Header } from "../../layout/header/header";
import { Footer } from "../../layout/footer/footer";

@Component({
  selector: 'app-post-job',
  imports: [Header, Footer],
  templateUrl: './post-job.html',
  styleUrl: './post-job.css',
})
export class PostJob {

}
