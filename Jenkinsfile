pipeline {
  agent any
  stages {
    stage('Init') {
      when { branch "PR*" }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform init
'''
      }
    }
    stage('Plan') {
      when { branch "PR*" }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan -out plan.tf
'''
        input 'Terraform Plan Approved?'
      }
    }
    stage('Apply') {
      when { branch "PR*" }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply plan.tf
'''
      }
    }
  }
}
