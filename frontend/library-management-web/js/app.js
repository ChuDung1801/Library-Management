const defaultUsers = [{id:'u-admin',name:'Nguyen Van Admin',username:'admin',password:'admin123',role:'ADMIN'},{id:'u-member',name:'Pham Minh Anh',username:'minhanh.pham',password:'user123',role:'MEMBER'}];
function getUsers(){try{return JSON.parse(localStorage.getItem('scholaris_users'))||defaultUsers}catch{return defaultUsers}}
function getCurrentUser(){const raw=sessionStorage.getItem('scholaris_user');return raw?JSON.parse(raw):null}
function showToast(message){alert(message)}
function borrowBook(){return {ok:true}}
