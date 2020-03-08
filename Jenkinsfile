pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform init
'''
      }
    }

    stage('Plan') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan -out plan.tf
'''
        input 'Terraform Plan Approved?'
      }
    }

    stage('Apply') {
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply plan.tf
'''
      }
    }

  }
}