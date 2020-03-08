pipeline {
  agent any
  stages {
    stage('Init') {
      when {
        expression { env.BRANCH.NAME.contains('PR') }
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform init
'''
      }
    }
    stage('Plan') {
      when {
        expression { env.BRANCH.NAME.contains('PR') }
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan -out plan.tf
'''
        input 'Terraform Plan Approved?'
      }
    }
    stage('Apply') {
      when {
        expression { env.BRANCH.NAME.contains('PR') }
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply plan.tf
'''
      }
    }
  }
}
