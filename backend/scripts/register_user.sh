#!/bin/sh

sh -c 'curl --insecure curl --header "Content-Type: multipart/form-data" https://localhost:3939/api/user/register?nickname=$1&email=$1_email@gmail.com&password=z12c3v4'
 
