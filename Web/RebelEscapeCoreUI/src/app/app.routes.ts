import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { PlayersListComponent } from './components/players-list/players-list.component';
import { GameBoardComponent } from './components/game-board/game-board.component';

export const routes: Routes = [{path: 'login', component: LoginComponent},
    { path: 'players-list', component: PlayersListComponent },
    { path: 'game-board', component: GameBoardComponent},
    { path: '**', component: LoginComponent },
];
