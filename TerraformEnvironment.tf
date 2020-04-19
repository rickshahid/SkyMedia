variable "ARM_TENANT_ID" {
 description = "Azure Active Directory (AAD) tenant identifier"
 type = string
}

variable "ARM_SUBSCRIPTION_ID" {
 description = "Azure service resources subscription identifier"
 type = string
}

variable "ARM_USE_MSI" {
 description = "Enables Azure Managed Service Identity (MSI)"
 type = string
}
