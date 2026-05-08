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
import { authGuard } from './core/auth/auth.guard';
import { roleGuard } from './core/auth/role.guard';
import { Unauthorized } from './unauthorized/unauthorized';
import { CandidateProfileComponent } from './features/candidate-profile/candidate-profile';


export const routes: Routes = [
    {path:'',component:Home},
    {path:'home',component:Home},
    {path:'jobs',component:Jobs},
    {path:'add-job',
     component:PostJob,
     canActivate: [authGuard,roleGuard],
     data: { roles: ['Employer', 'Admin'] }
    },
    {path:'companies',component:Companines, canActivate: [authGuard]},
    {path:'candidates',
     component:Candidates,
     canActivate: [authGuard,roleGuard],
     data: { roles: ['Candidate', 'Employer', 'Admin'] }
    },
    {path:'candidate-profile',
     component:CandidateProfileComponent,
     canActivate: [authGuard,roleGuard],
     data: { roles: ['Candidate', 'Admin'] }
    },
    {path:'candidate-profile/:id',
     component:CandidateProfileComponent,
     canActivate: [authGuard,roleGuard],
     data: { roles: ['Candidate', 'Admin'] }
    },
    {path:'dashboard',component:Dashboard, canActivate: [authGuard]},
    {path:'login',component:Login},
    {path:'register',component:Register},
    {path:'unauthorized',component:Unauthorized},
    {path:'job/:id',component:JobDetails, canActivate: [authGuard]},
    {path:'**',component:NotFound}
];
