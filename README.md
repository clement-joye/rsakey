# Description
Encrypt with private RSA key and decrypt with public key

# Generate RSA key

Open a terminal :  
> openssl genrsa -out pem_private.pem 1024  
> openssl rsa -in pem_private.pem -pubout > pem_public.pem

If openssl is not installed on your machine, use the one that comes with Git `C:\Program Files\Git\usr\bin>`