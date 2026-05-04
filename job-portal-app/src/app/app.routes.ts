import { Routes } from '@angular/router';
import { Home } from './home/home';
import { NotFound } from './not-found/not-found';
import { Jobs } from './features/jobs/jobs';
import { JobDetails } from './features/job-details/job-details';
import { PostJob } from './features/post-job/post-job';
import { Companines } from './features/companines/companines';
import { Candidates } from './features/candidates/candidates';
import { Dashboard } from './features/dashboard/dashboard';
import { Login } from './features/login/login';
import { Register } from './features/register/register';


export const routes: Routes = [
    {path:'',component:Home},
    {path:'home',component:Home},
    {path:'jobs',component:Jobs},
    {path:'add-job',component:PostJob},
    {path:'companies',component:Companines},
    {path:'candidates',component:Candidates},
    {path:'dashboard',component:Dashboard},
    {path:'login',component:Login},
    {path:'register',component:Register},
    {path:'job/:id',component:JobDetails},
    {path:'**',component:NotFound}
];
