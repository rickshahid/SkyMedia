variable "ARM_TENANT_ID" {
 description = "Azure Active Directory (AAD) tenant identifier"
 type = "string"
}

variable "ARM_SUBSCRIPTION_ID" {
 description = "Azure subscription identifier for resource deployment"
 type = "string"
}

variable "ARM_USE_MSI" {
 description = "Enables Azure Managed Service Identity (MSI) context"
 type = "boolean"
}
