import { createApp, createIdentityProvider } from '@kottster/server';
import schema from '../../kottster-app.json';
import { getEnvOrThrow } from '@kottster/common';

/* 
 * For security, consider moving the secret data to environment variables.
 * See https://kottster.app/docs/deploying#before-you-deploy
 */

const SECRET_KEY = getEnvOrThrow('SECRET_KEY');
const JWT_SECRET_SALT = getEnvOrThrow('JWT_SECRET_SALT');
const ROOT_USER_PASSWORD = getEnvOrThrow('ROOT_USER_PASSWORD');

export const app = createApp({
  schema,
  secretKey: '<your-secret-key-here>',

  
  /*
   * The identity provider configuration.
   * See https://kottster.app/docs/app-configuration/identity-provider
   */
  identityProvider: createIdentityProvider('sqlite', {
    fileName: 'app.db',

    passwordHashAlgorithm: 'bcrypt',
    jwtSecretSalt: JWT_SECRET_SALT,
    
    /* The root admin user credentials */
    rootUsername: 'admin',
    rootPassword: ROOT_USER_PASSWORD,
  }),
});
