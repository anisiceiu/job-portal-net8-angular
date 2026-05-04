import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, Input, OnInit, signal } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-jobs',
  imports: [CommonModule,RouterModule ],
  standalone:true,
  templateUrl: './jobs.html',
  styleUrl: './jobs.css',
})
export class Jobs implements OnInit{
@Input() limit: number=6;
@Input() showSearchBox: Boolean=true;

joblist = signal<Array<any>>([]);

 constructor(private http:HttpClient)
 {
   this.http.get<any>('https://localhost:7005/api/Jobs').subscribe(data=>{
    console.log(data);
    this.joblist.set(data);
   });
 }

 ngOnInit()
 {
    
 }
}
