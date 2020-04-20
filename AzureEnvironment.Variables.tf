variable "TF_VAR_tenant_id" {
    description = "Azure Active Directory (AAD) tenant identifier"
    type = string
}

variable "TF_VAR_subscription_id" {
    description = "Azure resources subscription identifier"
    type = string
}

variable "TF_VAR_use_msi" {
    description = "Azure Managed Service Identity (MSI)"
    default = false
}
