import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
registerForm : FormGroup;
 maxDate : Date;
 validationErrors: string[] = []

@Output() cancelRegister = new EventEmitter();


  constructor(private accountService: AccountService,private router: Router,
     private toastr: ToastrService, private fb: FormBuilder) { }

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }


  initializeForm(){
    this.registerForm= this.fb.group({
      gender : ['female'],
      username : ['',Validators.required],
      knownAs : ['',Validators.required],
      dateOfBirth : ['',Validators.required],
      city : ['',Validators.required],
      country : ['',Validators.required],
      password : ['',[Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValue('password')]]
    })
    this.registerForm.controls.password.valueChanges.subscribe(()=> {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    })
  }

  matchValue(matchTo: string): ValidatorFn{
    return (control: AbstractControl)=> {
      return control?.value  === control?.parent?.
      controls[matchTo].value? null:{isMatching: true}
    }
  }

  register(){
    this.accountService.register(this.registerForm.value).subscribe(response=> {
      this.router.navigateByUrl('members');
      this.toastr.success('registration completed successfully')
      this.cancel();
    }, error=> {
      this.validationErrors = error;
    });
  }

  cancel(){
    this.cancelRegister.emit(false)
  }
}
