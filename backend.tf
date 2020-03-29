terraform {
	backend "azurerm" {
		resource_group_name  = "Azure.Media.Studio-GitOps"
		storage_account_name = "gitops"
		container_name       = "terraform"
		key                  = "tfstate"
		use_msi              = true
	}
}
