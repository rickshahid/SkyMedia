variable "TF_VAR_ARM_TENANT_ID" {
    description = "Azure Active Directory (AAD) tenant identifier"
    type = string
}

variable "TF_VAR_ARM_SUBSCRIPTION_ID" {
    description = "Azure resources subscription identifier"
    type = string
}

variable "TF_VAR_ARM_USE_MSI" {
    description = "Azure Managed Service Identity (MSI)"
    default = false
}
