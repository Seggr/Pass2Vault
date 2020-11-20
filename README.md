# Pass2Vault
a interum solution for managing the local admin password on azure vms by automatically cycling password with Azure Keyvault.

### Flow
1. Checks to see if Vault Secrets will expire (to disable the rotation of a password simply remove it and in turn restore it.)
1. Creates a new super secret password that noone knows and stores it within keyvault.
1. Attempts to change the users password 
1. [maybe do some cleanup later of old secret versions] 
1. relax a little.